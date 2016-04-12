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

        internal CreateTable(MonoSQLiteManager dbManager, string tableName, Dictionary<string, ColumnInfo> columns): base(dbManager)
        {
            _tableName = tableName;
            _columns = columns;
        }

        public override void Run()
        {
            if(_dbManager.IsExistenceTable(_tableName))
            {
                Debug.Log("Exist Table => " + _tableName);

            }

            if (_columns == null)
            {
                _columns = new Dictionary<string, ColumnInfo> ();
                FieldInfo[] fields = _table.GetFields ();
                for (int i = 0; i < fields.Length; ++i) 
                {
                    string attributes = DBFieldAttribute.GetDBAttributes (fields[i]);
                    ColumnInfo c = new ColumnInfo (fields[i].Name, _dbManager.GetCTypeToSqlType(fields[i].FieldType), attributes);
                    _columns.Add (c._name, c);
                }
            }
            string q = "CREATE TABLE " + _tableName + " (";
            string q_fields = "";
            foreach (KeyValuePair<string, ColumnInfo> c in _columns)
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
            q+=" )";
            _dbManager.ExecuteNonQuery(q);
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

        public override void Run()
        {
            if (!_dbManager.IsExistenceTable(_tableName))
            {
                Debug.Log("not Exist Table => " + _tableName);
            }
            string q = "DELETE FROM "+_tableName;
            _dbManager.ExecuteNonQuery(q);
        }

        string _tableName;
    }

    public class DropTable : QueryContainer
    {
        internal DropTable(MonoSQLiteManager dbManager, string tableName) : base(dbManager)
        {
            _tableName = tableName;
        }

        public override void Run()
        {
            if (!_dbManager.IsExistenceTable(_tableName))
            {
                Debug.Log("Exist Table => " + _tableName);

            }

            string q = "PRAGMA foreign_keys";        
            _dbManager.ExecuteNonQuery(q);
            q = "PRAGMA foreign_keys = \"0\"";
            _dbManager.ExecuteNonQuery(q);
            q = "DROP TABLE " + "`"+_tableName+"`";
            _dbManager.ExecuteNonQuery(q);
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

        public override void Run()
        {
            string q = "CREATE TABLE "+_cloneName+" AS SELECT * FROM "+_originName;
            _dbManager.ExecuteNonQuery(q);
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

        public override void Run()
        {
            if (_dbManager.IsExistenceTableInField(_tableName, _columnName))
            {
                Debug.Log("Existence Field => " + _columnName);

            }
            string fieldType = _dbManager.GetCTypeToSqlType(_type);

            string q = "ALTER TABLE " + _tableName + " ADD COLUMN " + _columnName +" "+ fieldType;
            _dbManager.ExecuteNonQuery(q);
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

        public override void Run()
        {
            Dictionary<string, ColumnInfo> copyColumns = _dbManager.GetDBTableColumnsInfo (_toCopyTable);
            Dictionary<string, ColumnInfo> originColumns = _dbManager.GetDBTableColumnsInfo (_originName);

            foreach (KeyValuePair<string, ColumnInfo> c in copyColumns)
            {
                if (originColumns.ContainsKey (c.Key))
                {
                    if (originColumns [c.Key]._dataType != c.Value._dataType) 
                    {
                    }
                }
            }

            string insertQ = "INSERT INTO " + _toCopyTable + " ";
            string selectQ = " SELECT ";
            string columns = "";
            foreach (KeyValuePair<string, ColumnInfo> column in copyColumns)
            {
                if (columns.Length > 0)
                {   
                    columns += ",";
                }
                columns += "`";
                columns += column.Key;
                columns += "`";
            }
            selectQ += columns;
            selectQ +=" FROM " + "`"+_originName + "`";
            string q = insertQ + selectQ;
            _dbManager.ExecuteNonQuery(q);
        }
        string _originName;
        string _toCopyTable;
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

        public bool CommandQueries(bool isbackup = true)
		{
            string backupFileName = _filePath.Replace(_fileName, "_backup" + _fileName);
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

        public void CreateTable(string tableName, Dictionary<string, ColumnInfo> columns)
        {
            PushQuery(new DBAgent.CreateTable(this, tableName, columns));
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
                if (!CommandQueries(false))
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


