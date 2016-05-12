using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DBAgent;

namespace PatternSystem
{
    public class ResourcesPool
    {
        protected const string PATH = "/PatternSystem/Resources/DB/";
        protected const string DBFILENAME = "PatternSystem.db";
        protected Dictionary<System.Type, Dictionary<int, DBBaseTable>> _tables = new Dictionary<System.Type, Dictionary<int, DBBaseTable>>();
        private void LoadTable<T>()
                where T : DBBaseTable, new()
        {
            MonoSQLiteManager dbManager = new MonoSQLiteManager(PATH + DBFILENAME);
            List<T> table = dbManager.GetTableData<T>();
            dbManager.Close();
            Dictionary<int, DBBaseTable> tableMap = new Dictionary<int, DBBaseTable>();
            foreach (T ar in table)
            {
                tableMap.Add(ar.id, ar);
            }
            _tables.Add(typeof(T), tableMap);
        }
        public void LoadTables()
        {
            LoadTable<DBHabit>();
            LoadTable<DBTriger>();
            LoadTable<DBArrange>();
            LoadTable<DBTimer>();
            LoadTable<DBCall>();
            LoadTable<DBPhysicalData>();
            DataClerk.s_tables = _tables;
        }

        public Dictionary<int, DBBaseTable> GetTableData<T>()
            where T : DBBaseTable
        {
            return _tables[typeof(T)];
        }

        public T GetTableRow<T>(int id)
            where T : DBBaseTable, new()
        {
            Dictionary<int, DBBaseTable> table = GetTableData<T>();
            return table[id] as T;
        }
    }

}
