using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using DBAgent;

namespace PatternSystem
{	public class ResourcesPool
	{
        private const string PATH = "/PatternSystem/Resources/DB/PatternSystem.db";
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
			new EditorPrefabInfo(EditorPrefabList.HABIT, "Assets/PatternSystem/Editor/EditorPrefabs/Habit.prefab", typeof(DBHabit)),
            new EditorPrefabInfo(EditorPrefabList.TRIGER, "Assets/PatternSystem/Editor/EditorPrefabs/Triger.prefab", typeof(DBTriger)),
            new EditorPrefabInfo(EditorPrefabList.ARRANGE, "Assets/PatternSystem/Editor/EditorPrefabs/Arrange.prefab", typeof(DBArrange)),
            new EditorPrefabInfo(EditorPrefabList.MOVE, "Assets/PatternSystem/Editor/EditorPrefabs/Move.prefab", typeof(DBPhysicalData)),
            new EditorPrefabInfo(EditorPrefabList.SCALE, "Assets/PatternSystem/Editor/EditorPrefabs/Scale.prefab", typeof(DBPhysicalData)),
            new EditorPrefabInfo(EditorPrefabList.ROTATION, "Assets/PatternSystem/Editor/EditorPrefabs/Rotation.prefab", typeof(DBPhysicalData)),
            new EditorPrefabInfo(EditorPrefabList.ORBIT, "Assets/PatternSystem/Editor/EditorPrefabs/Orbit.prefab", typeof(DBPhysicalData)),
            new EditorPrefabInfo(EditorPrefabList.TIMER, "Assets/PatternSystem/Editor/EditorPrefabs/Timer.prefab", typeof(DBTimer)),
            new EditorPrefabInfo(EditorPrefabList.CALL, "Assets/PatternSystem/Editor/EditorPrefabs/Call.prefab", typeof(DBCall)),
		};
		Dictionary<EditorPrefabList, Object> _editorPrefabs = new Dictionary<EditorPrefabList, Object>();

        Dictionary<System.Type, Dictionary<int, DBBaseTable> > _tables = new Dictionary<System.Type, Dictionary<int, DBBaseTable> >();

		void Init()
		{
			for (int i = 0; i < _editorPrefabPaths.Length; ++i) 
			{
                Object o = AssetDatabase.LoadAssetAtPath(_editorPrefabPaths[i]._path, typeof(Object));
				_editorPrefabs.Add (_editorPrefabPaths [i]._key, o);
			}
            LoadTables();
		}

        [MenuItem("Tools/PatterSystem/CreateDBTable")]
        static private void CreateDBTable()
        {
            
            System.Type[] tables = {
                typeof( DBArrange ),
                typeof( DBHabit ),
                typeof( DBTriger ),
                typeof( DBTimer ),
                typeof( DBCall ),
                typeof( DBPhysicalData ),
            };

            TableCreator.PushTables(tables);
            MonoSQLiteManager dbManager = new MonoSQLiteManager(PATH);
            TableCreator.CreateTable(dbManager);
            dbManager.Close();
        }

        [MenuItem("Tools/PatterSystem/SaveCurrentPattern")]
        static private void SaveCurrentPattern()
        {
            MonoSQLiteManager dbManager = new MonoSQLiteManager(PATH);
            foreach (PatternSystem.HabitAgent ha in PatternSystem.HabitAgent.Habits)
            {
                if(ha.transform.parent != null)
                    ha.Save(dbManager);
            }

            dbManager.Close();
        }
		public Object GetEditorPrefab(EditorPrefabList key)
		{
			return _editorPrefabs [key];
		}

        private void LoadTable<T>()
            where T : DBBaseTable, new()
        {
            MonoSQLiteManager dbManager = new MonoSQLiteManager(PATH);
            List<T> table = dbManager.GetTableData<T>();
            dbManager.Close();
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

