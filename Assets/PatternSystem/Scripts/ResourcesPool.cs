using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using DBAgent;

namespace PatternSystem
{
	
	public class ResourcesPool
	{
		private static ResourcesPool s_instance;
		public static ResourcesPool Instance 
		{
			get
			{
				if (s_instance == null) 
				{
					s_instance = new ResourcesPool ();
					s_instance.Init ();
				}
				return s_instance;
			}
		}

		public struct EditorPrefabInfo
		{
			public EditorPrefabInfo(EditorPrefabList key, string path, System.Type tableDataType)
			{
				_key = key;
				_path = path;
				_tableDataType = tableDataType;
			}
			public EditorPrefabList 	_key;
			public string				_path;
			public System.Type			_tableDataType;
		}

		EditorPrefabInfo [] _editorPrefabPaths = new EditorPrefabInfo[]{
			new EditorPrefabInfo(EditorPrefabList.HABIT, "Assets/Editor/EditorPrefabs/Habit.prefab", typeof(DBHabit)),
			new EditorPrefabInfo(EditorPrefabList.TRIGER, "Assets/Editor/EditorPrefabs/Triger.prefab", typeof(DBTriger)),
			new EditorPrefabInfo(EditorPrefabList.ARRANGE, "Assets/Editor/EditorPrefabs/Arrange.prefab", typeof(DBArrange)),
			new EditorPrefabInfo(EditorPrefabList.MOVE, "Assets/Editor/EditorPrefabs/Move.prefab", typeof(DBPhysicalData)),
			new EditorPrefabInfo(EditorPrefabList.SCALE, "Assets/Editor/EditorPrefabs/Scale.prefab", typeof(DBPhysicalData)),
			new EditorPrefabInfo(EditorPrefabList.ROTATION, "Assets/Editor/EditorPrefabs/Rotation.prefab", typeof(DBPhysicalData)),
			new EditorPrefabInfo(EditorPrefabList.ORBIT, "Assets/Editor/EditorPrefabs/Orbit.prefab", typeof(DBPhysicalData)),
			new EditorPrefabInfo(EditorPrefabList.TIMER, "Assets/Editor/EditorPrefabs/Timer.prefab", typeof(DBTimer)),
			new EditorPrefabInfo(EditorPrefabList.CALL, "Assets/Editor/EditorPrefabs/Call.prefab", typeof(DBCall)),
		};
		Dictionary<EditorPrefabList, Object> _editorPrefabs = new Dictionary<EditorPrefabList, Object>();

        Dictionary<System.Type, Dictionary<int, DBBaseTable> > _tables = new Dictionary<System.Type, Dictionary<int, DBBaseTable> >();

        static DBAgent.MonoSQLiteManager _dbManager = null;

		void Init()
		{
			for (int i = 0; i < _editorPrefabPaths.Length; ++i) 
			{
                Object o = AssetDatabase.LoadAssetAtPath(_editorPrefabPaths[i]._path, typeof(Object));
				_editorPrefabs.Add (_editorPrefabPaths [i]._key, o);
			}
            _dbManager = new MonoSQLiteManager("/PatternSystem/Resources/DB/PatternSystem.db");
            LoadTables();
		}

        private void CreateDBTable()
        {
            MonoSQLiteManager _dbManager = new MonoSQLiteManager("/PatternSystem/Resources/DB/PatternSystem.db");
            System.Type[] tables = {
                typeof( DBArrange ),
                typeof( DBHabit ),
                typeof( DBTriger ),
                typeof( DBTimer ),
                typeof( DBCall ),
                typeof( DBPhysicalData ),
            };

            TableCreator.PushTables(tables);
            TableCreator.CreateTable(_dbManager);
        }
		public Object GetEditorPrefab(EditorPrefabList key)
		{
			return _editorPrefabs [key];
		}

        private void LoadTable<T>()
            where T : DBBaseTable, new()
        {
            List<T> table = _dbManager.GetTableData<T>();
            Dictionary<int, DBBaseTable> tableMap = new Dictionary<int, DBBaseTable>();
            foreach (T ar in table)
            {
                tableMap.Add(ar.id, ar);
            }
            _tables.Add(typeof(T),tableMap);
        }
        public void LoadTables()
        {
            LoadTable<DBArrange>();
            LoadTable<DBHabit>();
            LoadTable<DBTriger>();
            LoadTable<DBTimer>();
            LoadTable<DBCall>();
            LoadTable<DBPhysicalData>();
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

