using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using System.Reflection;

namespace DBAgent
{
    public struct ColumnInfo
    {
        internal ColumnInfo(string name, string dataType, string attribute)
        {
            _name = name;
            _dataType = dataType;
            _attribute = attribute;
        }

        public string _name;
        public string _dataType;
        public string _attribute;
    }

    public abstract class QueryContainer
	{
        protected QueryContainer(MonoSQLiteManager dbManager)
        {
            _dbManager = dbManager;
        }
        public abstract void Run();

        protected MonoSQLiteManager _dbManager = null;
	}

    public class CreateTable : QueryContainer
    {
        internal CreateTable(MonoSQLiteManager dbManager, System.Type table, bool isBackup = true) : base(dbManager)
        {
            _table = table;
            _tableName = table.Name;
            _isBackup = isBackup;
        }

		internal CreateTable(MonoSQLiteManager dbManager, string tableName, Dictionary<string, ColumnInfo> columns, bool isBackup = true): base(dbManager)
        {
            _tableName = tableName;
            _columns = columns;
			_isBackup = isBackup;
        }

        static public string GetQuery(MonoSQLiteManager dbManager, System.Type table)
        {   
            Dictionary<string, ColumnInfo> columns = new Dictionary<string, ColumnInfo>();
            FieldInfo[] fields = table.GetFields();
            for (int i = 0; i < fields.Length; ++i)
            {
                string attributes = DBFieldAttribute.GetDBAttributes(fields[i]);
                ColumnInfo c = new ColumnInfo(fields[i].Name, dbManager.GetCTypeToSqlType(fields[i].FieldType), attributes);
                columns.Add(c._name, c);
            }

            return GetQuery(dbManager, table.Name, columns);
        }

        static public string GetQuery(MonoSQLiteManager dbManager, string tableName, Dictionary<string, ColumnInfo> columns)
        {

            string q = "CREATE TABLE " + tableName + " (";
            string q_fields = "";
            foreach (KeyValuePair<string, ColumnInfo> c in columns)
            {
                if (q_fields.Length > 0)
                {
                    q_fields += ",";
                }

                q_fields += "`";
                q_fields += c.Key;
                q_fields += "`";
                q_fields += " ";
                q_fields += c.Value._dataType;
                q_fields += " ";
                q_fields += c.Value._attribute;
            }
            q += q_fields;
            q += " )";
            return q;
        }

        public override void Run()
        {            

            if (_columns == null)
            {
                _columns = _dbManager.GetTableColumnsInfo(_table);
            }

            if(_dbManager.IsExistenceTable(_tableName))
            {
                Debug.Log("Exist Table => " + _tableName);

                Dictionary<string, ColumnInfo> dbColumns = _dbManager.GetDBTableColumnsInfo(_tableName);

                if (dbColumns.Count == _columns.Count)
                {
                    bool isSame = true;
                    foreach(KeyValuePair<string, ColumnInfo> c in dbColumns)
                    {
                        if (_columns.ContainsKey(c.Key))
                        {
                            if (_columns[c.Key]._dataType != c.Value._dataType)
                            {
                                isSame = false;
                            }
                        }
                        else
                        {
                            isSame = false;
                        }
                    }

                    if (isSame)
                        return;
                }
                

                if(!_isBackup)
                {
                    _dbManager.ExecuteNonQuery(DropTable.GetQuery(_dbManager, _tableName));
                    _dbManager.ExecuteNonQuery(CreateTable.GetQuery(_dbManager, _tableName, _columns));
                }
                else
                {
                    string backupTableName = "_backup_" + _tableName;
                    _dbManager.ExecuteNonQuery(CloneTable.GetQuery(_dbManager, _tableName, backupTableName));                    
                    _dbManager.ExecuteNonQuery(DropTable.GetQuery(_dbManager, _tableName));
                    _dbManager.ExecuteNonQuery(CreateTable.GetQuery(_dbManager, _tableName, _columns));
                    _dbManager.ExecuteNonQuery(CopyTable.GetQuery(_dbManager, backupTableName, _tableName));
                    _dbManager.ExecuteNonQuery(DropTable.GetQuery(_dbManager, backupTableName));
                }   
            }
            else
            {
                _dbManager.ExecuteNonQuery(CreateTable.GetQuery(_dbManager, _tableName, _columns));
            }
        }

        System.Type _table = null;
        bool _isBackup;

        string _tableName;
        Dictionary<string, ColumnInfo> _columns = null;
    }

    public class DeleteTable : QueryContainer
    {
        internal DeleteTable(MonoSQLiteManager dbManager, string tableName) : base(dbManager)
        {
            _tableName = tableName;
        }

        static public string GetQuery(MonoSQLiteManager dbManaer, string tableName)
        {
            return "DELETE FROM " + tableName;
        }

        public override void Run()
        {
            if (!_dbManager.IsExistenceTable(_tableName))
            {
                Debug.Log("not Exist Table => " + _tableName);
            }            
            _dbManager.ExecuteNonQuery(GetQuery(_dbManager, _tableName));
        }

        string _tableName;
    }

    public class DropTable : QueryContainer
    {
        internal DropTable(MonoSQLiteManager dbManager, string tableName) : base(dbManager)
        {
            _tableName = tableName;
        }

        static public string GetQuery(MonoSQLiteManager dbManager, string tableName)
        {
            string q = "PRAGMA foreign_keys\n";
            q += "PRAGMA foreign_keys = \"0\";";
            q = "DROP TABLE " + "`"+tableName+"`";
            return q;
        }

        public override void Run()
        {
            if (!_dbManager.IsExistenceTable(_tableName))
            {
                Debug.Log("Exist Table => " + _tableName);

            }
            _dbManager.ExecuteNonQuery(GetQuery(_dbManager, _tableName));
        }

        string _tableName;
    }

    public class CloneTable : QueryContainer
    {
        internal CloneTable(MonoSQLiteManager dbManager, string originName, string cloneName) : base(dbManager)
        {
            _originName = originName;
            _cloneName = cloneName;
        }

        static public string GetQuery(MonoSQLiteManager dbManager, string originName, string cloneName)
        {
            return "CREATE TABLE " + cloneName + " AS SELECT * FROM " + originName;
        }

        public override void Run()
        {            
            _dbManager.ExecuteNonQuery(GetQuery(_dbManager, _originName, _cloneName));
        }

        string _originName;
        string _cloneName;
    }

    public class AddColumn : QueryContainer
    {
        internal AddColumn(MonoSQLiteManager dbManager, string tableName, string columnName, System.Type type) : base(dbManager)
        {
            _tableName = tableName;
            _type = type;
            _columnName = columnName;
        }

        static public string GetQuery(MonoSQLiteManager dbManager, string tableName, string columnName, System.Type type)
        {
            string fieldType = dbManager.GetCTypeToSqlType(type);
            return "ALTER TABLE " + tableName + " ADD COLUMN " + columnName + " " + fieldType;
        }

        public override void Run()
        {
            if (_dbManager.IsExistenceTableInField(_tableName, _columnName))
            {
                Debug.Log("Existence Field => " + _columnName);

            }            
            _dbManager.ExecuteNonQuery(GetQuery(_dbManager, _tableName, _columnName, _type));
        }

        string _tableName;
        string _columnName;
        System.Type _type;
    }

    public class CopyTable : QueryContainer
    {
        internal CopyTable(MonoSQLiteManager dbManager, string originName, string toCopyTable) : base(dbManager)
        {
            _originName = originName;
            _toCopyTable = toCopyTable;
        }

        static public string GetQuery(MonoSQLiteManager dbManager, string originName, string toCopyTable)
        {
            HashSet<string> SameDataType = new HashSet<string>();
            Dictionary<string, ColumnInfo> copyColumns = dbManager.GetDBTableColumnsInfo (toCopyTable);
            Dictionary<string, ColumnInfo> selectColumns = dbManager.GetDBTableColumnsInfo (originName);

            foreach (KeyValuePair<string, ColumnInfo> c in copyColumns)
            {
                if (selectColumns.ContainsKey (c.Key))
                {
                    if (selectColumns [c.Key]._dataType == c.Value._dataType) 
                    {
                        SameDataType.Add(c.Key);
                    }
                }
                /*
                if(c.Value._attribute.IndexOf(AUTOINCREMENT.Key) > 0 || 
                    c.Value._attribute.IndexOf(PRIMARY_KEY.Key) > 0)
                    notSameDataType.Add(c.Key);
                 * */
            }

            string insertQ = "INSERT INTO " + toCopyTable + " ";
            string selectQ = " SELECT ";
            string selectColumnsQ = "";
            string insertColumnsQ = "(";
            foreach (KeyValuePair<string, ColumnInfo> column in copyColumns)
            {
                if (SameDataType.Contains(column.Key))
                {
                    if (selectColumnsQ.Length > 0)
                    {
                        selectColumnsQ += ",";
                        insertColumnsQ += ",";
                    }

                    selectColumnsQ += "`";
                    selectColumnsQ += column.Key;
                    selectColumnsQ += "`";

                    insertColumnsQ += column.Key;
                }
            }
            insertColumnsQ += ") ";
            insertQ += insertColumnsQ;
            selectQ += selectColumnsQ;
            selectQ +=" FROM " + "`"+originName + "`";
            return insertQ + selectQ;
        }        

        public override void Run()
        {
            _dbManager.ExecuteNonQuery(GetQuery(_dbManager, _originName, _toCopyTable));
        }
        string  _originName;
        string  _toCopyTable;
    }

    public class InsertTable<T> : QueryContainer
    {
        internal InsertTable(MonoSQLiteManager dbManager, ref T table)
            : base(dbManager)
        {
            _table = table;
        }

        static public string GetQuery(MonoSQLiteManager dbManager, ref T table)
        {
            Dictionary<string, ColumnInfo> columns = dbManager.GetTableColumnsInfo(table.GetType());

            System.Type type = table.GetType();
            FieldInfo[] members = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetField | BindingFlags.GetField);            
            FieldInfo key = dbManager.GetPrimaryKey(members);

            string q = "INSERT INTO " + table.GetType().Name + " (";
            string columns_q = "";
            string value_q = " values(";

            for(int i = 0; i < members.Length; ++i)
            {
                if (members[i] == key)
                    continue;

                if(columns_q.Length > 0)
                {
                    columns_q += ",";
                    value_q += ",";
                }
                    
                columns_q += members[i].Name;
                if (members[i].FieldType == typeof(string) || members[i].FieldType == typeof(char))
                {
                    value_q += "'";
                    value_q += members[i].GetValue(table);
                    value_q += "'";
                }
                else
                {
                    value_q += members[i].GetValue(table).ToString();
                }
            }

            q += columns_q + ")";
            q += value_q + ")";
            return q;
        }

        public override void Run()
        {
            _dbManager.ExecuteNonQuery(GetQuery(_dbManager, ref _table));
        }
        T   _table;
    }


    public class UpdateTable<T> : QueryContainer
    {
        internal UpdateTable(MonoSQLiteManager dbManager, ref T table) : base(dbManager)
        {
            _table = table;
        }

        static public string GetQuery(MonoSQLiteManager dbManager, ref T table)
        {
            Dictionary<string, ColumnInfo> columns = dbManager.GetTableColumnsInfo(table.GetType());

            System.Type type = table.GetType();
            FieldInfo[] members = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetField | BindingFlags.GetField);
            FieldInfo key = dbManager.GetPrimaryKey(members);
            if (key == null)
            {
                Debug.Log("not Update None PrimaryKey table. name = " + type.Name);
                return null;
            }

            int keyindex = 0;
            for(int i = 0; i < members.Length; ++i)
            {
                if (key == members[i])
                {
                    keyindex = i;
                    break;
                }
            }


            string q = "UPDATE " + type.Name + " SET ";
            string columns_q = "";

            for (int i = 0; i < members.Length; ++i)
            {
                if (keyindex == i)
                    continue;
                if (columns_q.Length > 0)
                {
                    columns_q += ",";
                }

                columns_q += members[i].Name;
                columns_q += "=";
                if (members[i].FieldType == typeof(string) || members[i].FieldType == typeof(char))
                    columns_q += "`";
                columns_q += members[i].GetValue(table).ToString();
                if (members[i].FieldType == typeof(string) || members[i].FieldType == typeof(char))
                    columns_q += "`";
            }


            q += columns_q + " where "+members[keyindex].Name+"= "+members[keyindex].GetValue(table).ToString();
            return q;
        }
        public override void Run()
        {
            _dbManager.ExecuteNonQuery(GetQuery(_dbManager, ref _table));

        }

        T _table;
    }


    public class MonoSQLiteManager
	{
        string _fileName = null;
        string _filePath = null;
		private SqliteConnection _conn = null;
        public SqliteConnection Conn {get{return _conn; }}
		private Queue<QueryContainer> _queries = new Queue<QueryContainer>();

		private Dictionary<System.Type, string> _cTypeToSqlType = new Dictionary<System.Type, string>();
		private Dictionary<string, System.Type> _SqlTypeTocType = new Dictionary<string, System.Type>();

		public MonoSQLiteManager(string dbFilePath)
		{
            _filePath = Application.dataPath + dbFilePath;
            string path = "URI=file:" + _filePath;

            int slushfilenameIndex = path.LastIndexOf('/') + 1;
            int reslushfilenameIndex = path.LastIndexOf('\\') + 1;
            int filenameIndex = slushfilenameIndex > reslushfilenameIndex ? slushfilenameIndex : reslushfilenameIndex;

            _fileName = path.Substring(filenameIndex);
            
			_conn = new SqliteConnection(path);

			if(_conn != null)
			{            
				_conn.Open();            
			}
			else
			{
				Debug.Log("invalid file path : " + dbFilePath);
			}
			_cTypeToSqlType.Add(typeof(long), "INTEGER");
			_cTypeToSqlType.Add(typeof(int), "INTEGER");
			_cTypeToSqlType.Add(typeof(double), "REAL");
			_cTypeToSqlType.Add(typeof(float), "REAL");
			_cTypeToSqlType.Add(typeof(string), "TEXT");

			foreach(KeyValuePair<System.Type, string> cToSql in _cTypeToSqlType)
			{
				if(!_SqlTypeTocType.ContainsKey(cToSql.Value))
					_SqlTypeTocType.Add(cToSql.Value, cToSql.Key);
			}

			_SqlTypeTocType.Add("INT", typeof(int));
            _SqlTypeTocType.Add("varchar", typeof(string));
            _SqlTypeTocType.Add("VARCHAR", typeof(string));
		}

		~MonoSQLiteManager()
		{
			Close();
			_conn = null;
		}

		public void Close()
		{
			if (_conn != null)
			{
				_conn.Close();            
			}   
		}


		public string GetCTypeToSqlType(System.Type type)
		{
			if (!_cTypeToSqlType.ContainsKey(type))
				return "";
			return _cTypeToSqlType[type];
		}

		public System.Type GetSqlTypeToCType(string type)
		{
			if (!_SqlTypeTocType.ContainsKey(type))
				return null;
			return _SqlTypeTocType[type];
		}

        public System.Type FindSqlTypeToCType(string word)
        {
            foreach (KeyValuePair<string, System.Type> t in _SqlTypeTocType)
            {
                if (word.IndexOf(t.Key) >= 0)
                {
                    return t.Value;
                }
            }
            return null;
        }


        public void ExecuteNonQuery(string query)
		{
            SqliteCommand dbcmd = new SqliteCommand(query, _conn);
            dbcmd.ExecuteNonQuery();
            dbcmd.Dispose();
		}

		public SqliteDataReader Read(string query)
		{
			SqliteCommand dbcmd = new SqliteCommand(query, _conn);
            SqliteDataReader reader = dbcmd.ExecuteReader();
            dbcmd.Dispose();
            return reader;
		}

		public void PushQuery(QueryContainer query)
		{
			_queries.Enqueue(query);
		}

        public bool CommandQueries(bool isbackup = false)
		{
            string backupFileName = _filePath.Replace(_fileName, DateTime.Now.ToString("yyyy_mm_dd")+ "_" + _fileName);
			try
			{
                FileInfo file = new FileInfo(_filePath);
                if(file.Exists)
                {
                    if (isbackup)
                        file.CopyTo(backupFileName, true);
                    while (_queries.Count > 0)
                    {
                        QueryContainer q = _queries.Dequeue();
                        q.Run();
                    }
                }
                else
                {
                    Debug.Log(_filePath + " not create ");
                    _queries.Clear();
                    return false;
                }
				
			}
			catch(Exception e)
			{
				Debug.Log(e.ToString());
				Debug.Log("Both records are written to database.");
                _conn.Close();
                if (isbackup)
                {
                    FileInfo file = new FileInfo(backupFileName);
                    if (file.Exists)
                    {
                        file.CopyTo(_filePath, true);
                    }
                }
                string path = "URI=file:" + _filePath;
                _conn = new SqliteConnection(path);
                
				_queries.Clear();

                if (isbackup)
                    File.Delete(backupFileName);

				return false;
			}
            if (isbackup)
                File.Delete(backupFileName);
			return true;
		}


        public Dictionary<string, ColumnInfo> GetDBTableColumnsInfo(string tableName)
        {
            string q = "SELECT sql FROM sqlite_master WHERE name='" + tableName + "'";
            SqliteDataReader reader = Read(q);
            if (!reader.Read())
                return null;
            string tableQuery = reader.GetString(0);
            reader.Close();
            tableQuery = tableQuery.Replace("\"", "");
            tableQuery = tableQuery.Replace("`", "");
            tableQuery = tableQuery.Replace("'", "");
            tableQuery = tableQuery.Replace("\t", " ");
            tableQuery = tableQuery.Replace("\n", " ");


            int openIndex = tableQuery.IndexOf('(');
            int closeIndex = tableQuery.IndexOf(')');

            string columnsStr = tableQuery.Substring(openIndex + 1, closeIndex - openIndex-1);
            string[] columnsInfo = columnsStr.Split(',');

            Dictionary<string, ColumnInfo> columns = new Dictionary<string, ColumnInfo>();

            for (int i = 0; i < columnsInfo.Length; ++i)
            {
                string[] column = columnsInfo[i].Split(' ');
                List<string> columnInfo = new List<string>();
                for (int j = 0; j < column.Length; ++j )
                {
                    if(column[j] != "")
                    {
                        columnInfo.Add(column[j]);
                    }
                }

                ColumnInfo info = new ColumnInfo();
                info._name = columnInfo[0];
                info._dataType = GetCTypeToSqlType(FindSqlTypeToCType(columnsInfo[i]));
                if(info._dataType.Length == 0)
                    info._dataType = "TEXT";
                info._attribute = DBFieldAttribute.FindStringInAttributes(columnsInfo[i]);
                columns.Add(info._name, info);
            }
            return columns;
        }

		public Dictionary<string, ColumnInfo> GetTableColumnsInfo(System.Type table)
		{
			string tableName = table.Name;
			Dictionary<string, ColumnInfo> columns = new Dictionary<string, ColumnInfo> ();

			FieldInfo[] fields = table.GetFields ();
			for (int i = 0; i < fields.Length; ++i) 
			{
				string attributes = DBFieldAttribute.GetDBAttributes (fields[i]);
				ColumnInfo c = new ColumnInfo (fields[i].Name, GetCTypeToSqlType(fields[i].FieldType), attributes);
				columns.Add (c._name, c);
			}
			return columns;
		}

		public bool IsExistenceTable(string tableName)
		{
			string q = "SELECT COUNT(*) FROM sqlite_master WHERE name='" + tableName + "'";
			SqliteDataReader reader = Read(q);
            if (!reader.Read())
                return false;
			bool result = reader.GetInt32(0) == 1 ? true : false;

			reader.Close();
			return result;
		}

		public bool IsExistenceTableInField(string tableName, string fieldName)
		{		
            Dictionary<string, ColumnInfo> columns = GetDBTableColumnsInfo(tableName);
            if (columns == null)
                return false;
			return columns.ContainsKey(fieldName);
		}

        public void CreateTable(System.Type table, bool isBackup = true)
        {
            PushQuery(new DBAgent.CreateTable(this, table, isBackup));
        }

		public void CreateTable(string tableName, Dictionary<string, ColumnInfo> columns, bool isBackup = true)
        {
			PushQuery(new DBAgent.CreateTable(this, tableName, columns, isBackup));
        }

		public void DeleteTable(string tableName)
		{
            PushQuery(new DBAgent.DeleteTable(this, tableName));
		}

		public void DropTable(string tableName)
		{
            PushQuery(new DBAgent.DropTable(this, tableName));
		}

		public void AddColumn(string tableName, string columnName, System.Type type)
		{
            PushQuery(new DBAgent.AddColumn(this, tableName, columnName, type));
		}

        public void InsertTable<T>(ref T table)
        {
            PushQuery(new DBAgent.InsertTable<T>(this, ref table));
        }

        public void UpdateTable<T>(ref T table)
        {
            PushQuery(new DBAgent.UpdateTable<T>(this, ref table));
        }

        public T GetTableLastData<T>() where T : class, new()
        {
            System.Type type = typeof(T);
            FieldInfo[] members = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetField | BindingFlags.GetField);
            FieldInfo key = GetPrimaryKey(members);
            if(key != null)
            {
                string q = "SELECT * FROM "+type.Name+" WHERE "+key.Name+" = (SELECT MAX("+key.Name+")  FROM "+ type.Name+")";
                SqliteDataReader r = Read(q);
                T row = null;
                if(r.Read())
                {
                    row = new T();

                    for (int i = 0; i < members.Length; ++i)
                    {
                        for (int k = 0; k < r.FieldCount; ++k)
                        {
                            if (members[i].Name == r.GetName(k))
                            {
                                if (members[i].FieldType == typeof(int))
                                {
                                    if (!r.IsDBNull(k))
                                        members[i].SetValue(row, r.GetInt32(k));
                                }
                                else if (members[i].FieldType == typeof(float))
                                {
                                    if (!r.IsDBNull(k))
                                        members[i].SetValue(row, r.GetFloat(k));
                                }
                                else
                                {
                                    if (!r.IsDBNull(k))
                                        members[i].SetValue(row, r.GetString(k));
                                }
                            }
                        }
                    }
                }
                r.Close();
                return row;
            }
            else
            {
                List<T> rows = GetTableData<T>();
                if (rows != null)
                {
                    if (rows.Count > 0)
                        return rows[rows.Count - 1];
                }
            }
            
            return null;
        }

		public List<T> GetTableData<T>() 
            where T : class, new()
		{
			System.Type type = typeof(T);
			string tableName = type.Name;
			if (IsExistenceTable (tableName))
			{
				List<T> table = new List<T>();

				FieldInfo[] members = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetField | BindingFlags.GetField);
                FieldInfo key = GetPrimaryKey(members);

                string q = "select rowid, * from " + tableName;
                if (key != null)
                {
                    for(int i = 0; i < members.Length; ++i)
                    {
                        if (key == members[i])
                        {
                            q += " order by";
                            q += " " + members[i].Name + " asc";
                            break;
                        }
                    }
                }

				SqliteDataReader reader = Read (q);
            
				while (reader.Read ())
				{
					var row = new T();

					for (int i = 0; i < members.Length; ++i) 	
					{
						for (int k = 0; k < reader.FieldCount; ++k)
						{
							if (members [i].Name == reader.GetName (k))
                            {
                                if (members[i].FieldType == typeof(int))
                                {
                                    if(!reader.IsDBNull(k))
                                        members [i].SetValue (row, reader.GetInt32(k));
                                }
                                else if (members[i].FieldType == typeof(float))
                                {
                                    if(!reader.IsDBNull(k))
                                        members [i].SetValue (row, reader.GetFloat(k));
                                }
                                else
                                {
                                    if(!reader.IsDBNull(k))
                                        members[i].SetValue(row, reader.GetString(k));
                                }
							}
						}
					}
					table.Add (row);
				}
				reader.Close ();
				return table;
			}
			return null;
		}

        public SortedDictionary<int, T> GetTableDataToMap<T>()
            where T : class, new()
        {
            System.Type type = typeof(T);
            string tableName = type.Name;
            if (IsExistenceTable (tableName))
            {
                SortedDictionary<int, T> table = new SortedDictionary<int, T>();

                FieldInfo[] members = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetField | BindingFlags.GetField);
                FieldInfo key = GetPrimaryKey(members);
                if (key == null)
                {
                    Debug.Log("None Primary Key " + tableName + " !!!");
                    return null;
                }

                int keyindex = 0;
                for(int i = 0; i < members.Length; ++i)
                {
                    if (key == members[i])
                    {
                        keyindex = i;
                        break;
                    }
                }

                
                string q = "select rowid, * from " + tableName;

                SqliteDataReader reader = Read (q);

                while (reader.Read ())
                {
                    var row = new T();

                    for (int i = 0; i < members.Length; ++i)    
                    {
                        for (int k = 0; k < reader.FieldCount; ++k)
                        {
                            if (members [i].Name == reader.GetName (k))
                            {
                                if (members[i].FieldType == typeof(int))
                                {
                                    if(!reader.IsDBNull(k))
                                        members [i].SetValue (row, reader.GetInt32(k));
                                }
                                else if (members[i].FieldType == typeof(float))
                                {
                                    if(!reader.IsDBNull(k))
                                        members [i].SetValue (row, reader.GetFloat(k));
                                }
                                else
                                {
                                    if(!reader.IsDBNull(k))
                                        members[i].SetValue(row, reader.GetString(k));
                                }
                            }
                        }
                    }
                    var rowid = members[keyindex].GetValue(row);

                    table.Add (int.Parse(rowid.ToString()),row);

                }
                reader.Close ();
                return table;
            }
            return null;
        }

        public FieldInfo GetPrimaryKey(FieldInfo[] fields)
        {
            foreach (FieldInfo field in fields)
            {
                string attributes = DBFieldAttribute.GetDBAttributes(field);
                if (attributes.IndexOf(PRIMARY_KEY.Key) >= 0)
                {
                    return field;
                }
                
            }
            return null;
        }

		
		public bool DQDeleteColumn(string tableName, string deleteColumnName)
		{   
            Dictionary<string, ColumnInfo> dbColumns = GetDBTableColumnsInfo (tableName);
            if(dbColumns != null)
            {
                if (dbColumns.ContainsKey(deleteColumnName))
                    return false;

                dbColumns.Remove(deleteColumnName);
                string backupTableName = "_backup_" + tableName;
                CloneTable(tableName, backupTableName);
                DropTable(tableName);                
                CreateTable(tableName, dbColumns);
                if (!CommandQueries())
                {   
                    return false;
                }

                CopyTable(backupTableName, tableName);
                DropTable(backupTableName);

                return CommandQueries();
            }
            else
            {
                Debug.Log("not Exist Table => " + tableName);
            }
            return false;
		}



        public void CopyTable(string originName, string toCopyTable)
        {
            PushQuery(new DBAgent.CopyTable(this, originName, toCopyTable));
        }


		public bool IsTypeCompare(string tableName, string columnName, System.Type type)
		{
			if (IsExistenceTableInField (tableName, columnName))
			{
                Dictionary<string, ColumnInfo> columns = GetDBTableColumnsInfo (tableName);
                return columns [columnName]._dataType == GetCTypeToSqlType(type) ? true : false;
			}

			return false;
		}


        public void CloneTable(string originName, string newName)
		{
            PushQuery(new DBAgent.CloneTable(this, originName, newName));
		}
	}
	
}


