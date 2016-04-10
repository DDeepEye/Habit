using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using System.Reflection;

namespace DBAgent
{
	public struct QueryContainer
	{
		internal QueryContainer(string query)
		{
			_query = query;
			_query += ";";
		}
		private string _query;
		public string Query { get { return _query; } }
	}

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

	public class MonoSQLiteManager
	{
		private SqliteConnection _conn = null;
		private SqliteTransaction _trans = null;
		private Queue<QueryContainer> _queries = new Queue<QueryContainer>();

		private Dictionary<System.Type, string> _cTypeToSqlType = new Dictionary<System.Type, string>();
		private Dictionary<string, System.Type> _SqlTypeTocType = new Dictionary<string, System.Type>();

		public MonoSQLiteManager(string dbFilePath)
		{
			string path = "URI=file:" + Application.dataPath + dbFilePath;
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


		private int ExecuteNonQuery(string query)
		{
			if(_trans != null)
			{
				Debug.Log("Lock Trans!!");
				return 0;
			}
			SqliteCommand dbcmd = new SqliteCommand(query, _conn);
			int r = dbcmd.ExecuteNonQuery();
			dbcmd.Dispose();
			return r;
		}

		private SqliteDataReader Read(string query)
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

		public bool CommandQueries()
		{
			SqliteCommand command = new SqliteCommand(_conn);
			_trans = _conn.BeginTransaction();
			command.Transaction = _trans;
			try
			{   
				while (_queries.Count > 0)
				{
					command.CommandText = _queries.Dequeue().Query;
					command.ExecuteNonQuery();
				}
				_trans.Commit();
			}
			catch(Exception e)
			{
				command.Cancel();
				_trans.Rollback();
				Debug.Log(e.ToString());
				Debug.Log("Both records are written to database.");
				_trans = null;
				command = null;
				_queries.Clear();
				return false;
			}
			_trans = null;
			command = null;

			return true;
		}

		public Dictionary<string, System.Type> GetDBTableColumnsInfo(string tableName)
		{
			string q = "SELECT sql FROM sqlite_master WHERE name='" + tableName + "'";
			SqliteDataReader reader = Read(q);
			reader.Read();
			string tableQuery = reader.GetString(0);
			reader.Close();
			tableQuery.Replace("\"", "");
			tableQuery.Replace("`", "");
			tableQuery.Replace("'", "");
			tableQuery.Replace("\t", " ");
			tableQuery.Replace("\n", " ");

			int openIndex = tableQuery.IndexOf('(');
			int closeIndex = tableQuery.IndexOf(')');

			string columnsStr = tableQuery.Substring(openIndex + 1, closeIndex - openIndex-1);
			string[] columnsInfo = columnsStr.Split(',');

			Dictionary<string, System.Type> columns = new Dictionary<string, System.Type>();

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
				switch (columnInfo.Count)
				{
				case 2:
					columns.Add(columnInfo[0], GetSqlTypeToCType(columnInfo[1]));
					break;
				case 1:
					columns.Add(columnInfo[0], typeof(string));
					break;
				}
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
			reader.Read();
			bool result = reader.GetInt32(0) == 1 ? true : false;

			reader.Close();
			return result;
		}

		public bool IsExistenceTableInField(string tableName, string fieldName)
		{		
			Dictionary<string, System.Type> columns = GetDBTableColumnsInfo(tableName);
			return columns.ContainsKey(fieldName);
		}

		public int CreateTable(System.Type table, bool isBackup = true)
		{
			string tableName = table.Name;
			if(IsExistenceTable(tableName))
			{
				Debug.Log("Exist Table => " + tableName);
				return 0;
			}
			Dictionary<string, ColumnInfo> columns = new Dictionary<string, ColumnInfo> ();

			FieldInfo[] fields = table.GetFields ();
			for (int i = 0; i < fields.Length; ++i) 
			{
				string attributes = DBFieldAttribute.GetDBAttributes (fields[i]);
				ColumnInfo c = new ColumnInfo (fields[i].Name, GetCTypeToSqlType(fields[i].FieldType), attributes);
				columns.Add (c._name, c);
			}
			CreateTable (tableName, columns);
			return 1;
		}

		private int CreateTable(string tableName, Dictionary<string, ColumnInfo> columns)
		{
			string q = "CREATE TABLE " + tableName + " (";
			string fields = "";
			foreach (KeyValuePair<string, ColumnInfo> c in columns)
			{
				if (fields.Length > 0)
				{
					fields += ",";
				}

				fields += "`";
				fields += c.Key;
				fields += "`";
				fields += " ";
				fields += c.Value._dataType;
				fields += " ";
				fields += c.Value._attribute;
			}
			q += fields;
			q+=" )";
			return ExecuteNonQuery(q);
		}

		private string GetQueryCreateTable(string tableName, Dictionary<string, ColumnInfo> columns)
		{
			string q = "CREATE TABLE " + tableName + " (";
			string strFields = "";
			foreach (KeyValuePair<string, ColumnInfo> c in columns)
			{
				if (strFields.Length > 0)
				{
					strFields += ",";
				}

				strFields += "`";
				strFields += c.Key;
				strFields += "`";
				strFields += " ";
				strFields += c.Value._dataType;
				strFields += " ";
				strFields += c.Value._attribute;
			}
			q += strFields;
			q+=" )";
			return q;
		}

		public void PushQueryCreateTable(System.Type table, bool isBackup = true)
		{
			string tableName = table.Name;
			if(IsExistenceTable(tableName))
			{
				Debug.Log("Exist Table => " + tableName);
			}
			Dictionary<string, ColumnInfo> columns = GetTableColumnsInfo(table);
			PushQuery (new QueryContainer (GetQueryCreateTable (tableName, columns)));
		}

		public int CreateTable(string tableName, Dictionary<string, System.Type> columns, string idColumnName = null, System.Type idtype = null)
		{
			if(IsExistenceTable(tableName))
			{
				Debug.Log("Exist Table => " + tableName);
				return 0;
			}
			string q = "CREATE TABLE " + tableName + " (";
			if (idColumnName != null)
			{
				if (idColumnName.Length > 0)
				{
					q += "`" + idColumnName + "`";
					if (idtype != null)
						q += " " + GetCTypeToSqlType (idtype);
					else
						q += " " + GetCTypeToSqlType (typeof(int));
					q += " NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE";

				}
			}

			if (columns != null)
			{
				if (columns.Count > 0)
				{
					int columnCount = 0;
					foreach (KeyValuePair<string, System.Type> column in columns) 
					{
						if(columnCount != 0)
							q += ", ";
						q += "`"+column.Key+"`"+" "+GetCTypeToSqlType(column.Value);
						++columnCount;
					}
				}
			}

			q += ")";
			return ExecuteNonQuery(q);
		}

		public void PushQueryCreateTable(string tableName, Dictionary<string, System.Type> columns, string idColumnName = null, System.Type idtype = null)
		{
			if (IsExistenceTable(tableName))
			{
				Debug.Log("Exist Table => " + tableName);
			}
			string q = "CREATE TABLE " + tableName + " (";
			if (idColumnName != null)
			{
				if (idColumnName.Length > 0)
				{
					q += "`" + idColumnName + "`";
					if (idtype != null)
						q += " " + GetCTypeToSqlType(idtype);
					else
						q += " " + GetCTypeToSqlType(typeof(int));
					q += " NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE";
				}
			}

			if (columns != null)
			{
				if (columns.Count > 0)
				{
					int columnCount = 0;
					foreach (KeyValuePair<string, System.Type> column in columns)
					{
						if (columnCount != 0)
							q += ", ";
						q += "`" + column.Key + "`" + " " + GetCTypeToSqlType(column.Value);
						++columnCount;
					}
				}
			}

			q += ")";
			PushQuery (new QueryContainer (q));
		}

		public int DeleteTable(string tableName)
		{
			if (!IsExistenceTable(tableName))
			{
				Debug.Log("Exist Table => " + tableName);
				return 0;
			}
			string q = "DELETE FROM "+tableName;
			return ExecuteNonQuery(q);
		}

		public void PushQueryDeleteTable(string tableName)
		{
			if (!IsExistenceTable(tableName))
			{
				Debug.Log("Exist Table => " + tableName);
			}
			string q = "DELETE FROM " + tableName;
			PushQuery (new QueryContainer(q));
		}

		public int DropTable(string tableName)
		{
			if (!IsExistenceTable(tableName))
			{
				Debug.Log("Exist Table => " + tableName);
				return 0;
			}

			string q = "PRAGMA foreign_keys";        
			ExecuteNonQuery(q);
			q = "PRAGMA foreign_keys = \"0\"";
			ExecuteNonQuery(q);
			q = "DROP TABLE IF EXISTS " + "`"+tableName+"`";        
			return ExecuteNonQuery (q);
		}

		public void PushQueryDropTable(string tableName)
		{
			if (!IsExistenceTable(tableName))
			{
				Debug.Log("Exist Table => " + tableName);
			}

			string q = "PRAGMA foreign_keys";
			PushQuery(new QueryContainer(q));
			q = "PRAGMA foreign_keys = \"0\"";
			PushQuery(new QueryContainer(q));
			q = "DROP TABLE " + "`" + tableName + "`";
			PushQuery (new QueryContainer (q));
		}



		public int AddColumn(string tableName, string fieldName, System.Type type)
		{
			if (IsExistenceTableInField(tableName, fieldName))
			{
				Debug.Log("Existence Field => " + fieldName);
				return 0;
			}

			string fieldType = "INTEGER";
			switch(type.ToString())
			{
			case "System.Single":
			case "System.Double":
				fieldType = "REAL";
				break;
			case "System.String":
				fieldType = "TEXT";
				break;
			}
			string q = "ALTER TABLE " + tableName + " ADD COLUMN " + fieldName +" "+ fieldType;
			return ExecuteNonQuery(q);
		}

		public void PushQueryAddColumn(string tableName, string fieldName, System.Type type)
		{
			if (IsExistenceTableInField(tableName, fieldName))
			{
				Debug.Log("Existence Field => " + fieldName);            
			}

			string fieldType = "INTEGER";
			switch (type.ToString())
			{
			case "System.Single":
			case "System.Double":
				fieldType = "REAL";
				break;
			case "System.String":
				fieldType = "TEXT";
				break;
			}
			string q = "ALTER TABLE " + tableName + " ADD COLUMN " + fieldName + " " + fieldType;
			PushQuery (new QueryContainer (q));
		}

		public int AddColumntoInt(string tableName, string fieldName)
		{
			if (IsExistenceTableInField(tableName, fieldName))
			{
				Debug.Log("Existence Table in Field!!! => " + tableName + ", Field => " + fieldName);
				return 0;
			}
			string q = "ALTER TABLE " + tableName + " ADD COLUMN " + fieldName +" INTEGER";
			return ExecuteNonQuery(q);
		}

		public void PushQueryAddColumntoInt(string tableName, string fieldName)
		{
			if (IsExistenceTableInField(tableName, fieldName))
			{
				Debug.Log("Existence Table in Field!!! => " + tableName + ", Field => " + fieldName);
			}
			string q = "ALTER TABLE " + tableName + " ADD COLUMN " + fieldName + " INTEGER";
			PushQuery(new QueryContainer(q));
		}

		public int AddColumntoReal(string tableName, string fieldName)
		{
			if (IsExistenceTableInField(tableName, fieldName))
			{
				Debug.Log("Existence Table in Field!!! => " + tableName + ", Field => " + fieldName);
				return 0;
			}
			string q = "ALTER TABLE " + tableName + " ADD COLUMN " + fieldName +" REAL";
			return ExecuteNonQuery(q);
		}

		public void PushQueryAddColumntoReal(string tableName, string fieldName)
		{
			if (IsExistenceTableInField(tableName, fieldName))
			{
				Debug.Log("Existence Table in Field!!! => " + tableName + ", Field => " + fieldName);
			}
			string q = "ALTER TABLE " + tableName + " ADD COLUMN " + fieldName + " REAL";
			PushQuery(new QueryContainer(q));
		}

		public int AddColumntoString(string tableName, string fieldName, int size = 0)
		{
			if (IsExistenceTableInField(tableName, fieldName))
			{
				Debug.Log("Existence Table in Field!!! => " + tableName + ", Field => " + fieldName);
				return 0;
			}
			string q = "ALTER TABLE " + tableName + " ADD COLUMN " + fieldName +" ";
			if (size > 0) 
				q += "varchar("+size.ToString()+")";
			else
				q += "TEXT";
			return ExecuteNonQuery(q);
		}

		public void PushQueryAddColumntoString(string tableName, string fieldName, int size = 0)
		{
			if (IsExistenceTableInField(tableName, fieldName))
			{
				Debug.Log("Existence Table in Field!!! => " + tableName + ", Field => " + fieldName);
			}
			string q = "ALTER TABLE " + tableName + " ADD COLUMN " + fieldName + " ";
			if (size > 0)
				q += "varchar(" + size.ToString() + ")";
			else
				q += "TEXT";
			PushQuery(new QueryContainer(q));
		}

		public int DeleteColumn(string tableName, string deleteColumnName)
		{
			if (!IsExistenceTableInField(tableName, deleteColumnName))
			{
				Debug.Log("Existence Table not in Column!!! => " + tableName + ", Column => " + deleteColumnName);
				return 0;
			}

			string backupTableName = "_backup_" + tableName;
			Dictionary<string, System.Type> dbColumns = GetDBTableColumnsInfo (tableName);

			PushQueryTableClone(tableName, backupTableName);
			PushQueryDropTable(tableName);
			dbColumns.Remove (deleteColumnName);
			PushQueryCreateTable(tableName, dbColumns);

            PushQueryDBTableCopy(backupTableName, tableName);
			PushQueryDropTable(backupTableName);
			if (!CommandQueries())
				return 0;
			return 1;
		}

        private void PushQueryDBTableCopy(string originName, string toCopyTable)
        {
            Dictionary<string, System.Type> copyColumns = GetDBTableColumnsInfo (toCopyTable);
            Dictionary<string, System.Type> originColumns = GetDBTableColumnsInfo (originName);

            foreach (KeyValuePair<string, System.Type> c in copyColumns)
            {
                if (originColumns.ContainsKey (c.Key))
                {
                    if (originColumns [c.Key] != c.Value) 
                    {
                    }
                }

            }

            string insertQ = "INSERT INTO " + toCopyTable + " ";
            string selectQ = " SELECT ";
            foreach (KeyValuePair<string, System.Type> column in copyColumns)
            {
                if (selectQ.Length > 0)
                {   
                    selectQ += ",";
                }
                selectQ += "`";
                selectQ += column.Key;
                selectQ += "`";
            }
            selectQ +=" FROM " + "`"+originName + "`";
            string q = insertQ + selectQ;
            PushQuery (new QueryContainer (q));
        }

		public bool IsTypeCompare(string tableName, string columnName, System.Type type)
		{
			if (IsExistenceTableInField (tableName, columnName))
			{
				Dictionary<string, System.Type> columns = GetDBTableColumnsInfo (tableName);
				return columns [columnName] == type ? true : false;
			}

			return false;
		}


		public bool TableClone(string originName, string newName)
		{
			if (!IsExistenceTable(originName))
			{
				Debug.Log("Not Exist Table => " + originName);
				return false;
			}

			if (originName == null || newName == null)
			{
				Debug.Log("table name is null !!");
				return false;
			}

			if (originName.Length == 0 || newName.Length == 0)
			{
				Debug.Log("table name is length zero !!");
				return false;
			}


			string q = "CREATE TABLE "+newName+" AS SELECT * FROM "+originName;
			ExecuteNonQuery(q);
			return true;
		}

		public void PushQueryTableClone(string originName, string newName)
		{
			if (!IsExistenceTable(originName))
			{
				Debug.Log("Not Exist Table => " + originName);
			}

			if (originName == null || newName == null)
			{
				Debug.Log("table name is null !!");
			}

			if (originName.Length == 0 || newName.Length == 0)
			{
				Debug.Log("table name is length zero !!");
			}

			string q = "CREATE TABLE " + newName + " AS SELECT * FROM " + originName;
			PushQuery(new QueryContainer(q));
		}

		public void TableidFree(string tableName, string idName)
		{
			if (!IsExistenceTableInField(tableName, idName))
				return;

			string q = "CREATE TABLE `sqlitebrowser_rename_column_new_table` (`"+idName+"`	INTEGER) ";
			ExecuteNonQuery(q);         
			q = "INSERT INTO sqlitebrowser_rename_column_new_table SELECT `"+idName+"` FROM "+tableName;
			ExecuteNonQuery(q);
			q = "PRAGMA foreign_keys";
			ExecuteNonQuery(q);
			q = "PRAGMA foreign_keys = \"0\"";
			ExecuteNonQuery(q);
			q = "DROP TABLE "+tableName;
			ExecuteNonQuery(q);
			q = "ALTER TABLE `sqlitebrowser_rename_column_new_table` RENAME TO " + tableName;
			ExecuteNonQuery(q);
			q = "SELECT type,name,sql,tbl_name FROM sqlite_master UNION SELECT type,name,sql,tbl_name FROM sqlite_temp_master";
			ExecuteNonQuery(q);
			q = "PRAGMA foreign_keys = \"0\"";
			ExecuteNonQuery(q);
			q = "SELECT type,name,sql,tbl_name FROM sqlite_master UNION SELECT type,name,sql,tbl_name FROM sqlite_temp_master";
			ExecuteNonQuery(q);
			q = "SELECT type,name,sql,tbl_name FROM sqlite_master UNION SELECT type,name,sql,tbl_name FROM sqlite_temp_master";
			ExecuteNonQuery(q);
			q = "SELECT COUNT(*) FROM (SELECT `_rowid_`,* FROM `sample` ORDER BY `_rowid_` ASC)";
			ExecuteNonQuery(q);
			q = "SELECT `_rowid_`,* FROM `sample` ORDER BY `_rowid_` ASC LIMIT 0, 50000";
			ExecuteNonQuery(q);
			q = "SELECT COUNT(*) FROM (SELECT `_rowid_`,* FROM `sample` ORDER BY `_rowid_` ASC)";
			ExecuteNonQuery(q);
			q = "SELECT `_rowid_`,* FROM `sample` ORDER BY `_rowid_` ASC LIMIT 0, 50000";
			ExecuteNonQuery(q);
		}

		public void TableidOptionAdd(string tableName, string idName)
		{
			if (!IsExistenceTableInField(tableName, idName))
				return;

			string q = "CREATE TABLE `sqlitebrowser_rename_column_new_table` (`"+idName+"`	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE) ";
			ExecuteNonQuery(q);
			q = "INSERT INTO sqlitebrowser_rename_column_new_table SELECT `"+idName+"` FROM " + tableName;
			ExecuteNonQuery(q);
			q = "PRAGMA foreign_keys";
			ExecuteNonQuery(q);
			q = "PRAGMA foreign_keys = \"0\"";
			ExecuteNonQuery(q);
			q = "DROP TABLE " + tableName;
			ExecuteNonQuery(q);
			q = "ALTER TABLE `sqlitebrowser_rename_column_new_table` RENAME TO " + tableName;
			ExecuteNonQuery(q);
			q = "SELECT type,name,sql,tbl_name FROM sqlite_master UNION SELECT type,name,sql,tbl_name FROM sqlite_temp_master";
			ExecuteNonQuery(q);
			q = "PRAGMA foreign_keys = \"0\"";
			ExecuteNonQuery(q);
			q = "SELECT type,name,sql,tbl_name FROM sqlite_master UNION SELECT type,name,sql,tbl_name FROM sqlite_temp_master";
			ExecuteNonQuery(q);
			q = "SELECT type,name,sql,tbl_name FROM sqlite_master UNION SELECT type,name,sql,tbl_name FROM sqlite_temp_master";
			ExecuteNonQuery(q);
			q = "SELECT COUNT(*) FROM (SELECT `_rowid_`,* FROM `sample` ORDER BY `_rowid_` ASC)";
			ExecuteNonQuery(q);
			q = "SELECT `_rowid_`,* FROM `sample` ORDER BY `_rowid_` ASC LIMIT 0, 50000";
			ExecuteNonQuery(q);
			q = "SELECT COUNT(*) FROM (SELECT `_rowid_`,* FROM `sample` ORDER BY `_rowid_` ASC)";
			ExecuteNonQuery(q);
			q = "SELECT `_rowid_`,* FROM `sample` ORDER BY `_rowid_` ASC LIMIT 0, 50000";
			ExecuteNonQuery(q);
		}

	}
	
}


