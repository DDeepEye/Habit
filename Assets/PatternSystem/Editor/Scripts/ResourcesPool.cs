using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using DBAgent;

namespace PatternSystem
{	public class ResourcesPool
	{
        private const string PATH = "/PatternSystem/Resources/DB/";
        private const string DBFILENAME = "PatternSystem.db";
		private static ResourcesPool s_instance;
        private static GameObject s_patternList;
        public static GameObject PatternList
        {
            get
            {
                s_patternList = GameObject.Find("PatternList");
                if (s_patternList == null)
                    s_patternList = new GameObject("PatternList");
                
                return s_patternList;
            }
        }
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
			public string			    _path;
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
        Dictionary<EditorPrefabList,UnityEngine.Object> _editorPrefabs = new Dictionary<EditorPrefabList, UnityEngine.Object>();

        Dictionary<System.Type, Dictionary<int, DBBaseTable> > _tables = new Dictionary<System.Type, Dictionary<int, DBBaseTable> >();

		void Init()
		{
			for (int i = 0; i < _editorPrefabPaths.Length; ++i) 
			{
                UnityEngine.Object o = AssetDatabase.LoadAssetAtPath(_editorPrefabPaths[i]._path, typeof(UnityEngine.Object));
				_editorPrefabs.Add (_editorPrefabPaths [i]._key, o);
			}
            DataClerk.s_editorPrefabs = _editorPrefabs;
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
            MonoSQLiteManager dbManager = new MonoSQLiteManager(PATH + DBFILENAME);
            TableCreator.CreateTable(dbManager);
            dbManager.Close();
        }

        [MenuItem("Tools/PatterSystem/SaveCurrentPattern")]
        static private void SaveCurrentPattern()
        {
            MonoSQLiteManager dbManager = new MonoSQLiteManager(PATH + DBFILENAME);

            string backupFileName = PATH + DateTime.Now.ToString("yyyy_mm_dd")+ ".db";
            FileInfo file = new FileInfo(PATH + DBFILENAME);
            if(file.Exists)
            {
                file.CopyTo(backupFileName, true);
            }

            foreach (PatternSystem.HabitAgent ha in PatternSystem.HabitAgent.Habits)
            {
                if(ha.transform.parent != null)
                {
                    if(!ha.Save(dbManager))
                    {
                        FileInfo bfile = new FileInfo(backupFileName);
                        if(bfile.Exists)
                        {
                            bfile.CopyTo(PATH + DBFILENAME);
                            break;
                        }
                    }
                }
            }
            file.Delete();

            dbManager.Close();
        }
        [MenuItem("Tools/PatterSystem/ReadPattern")]
        static private void ReadPattern()
        {
            HabitAgent.s_listManager = PatternList;

            Type habitType = Instance._editorPrefabPaths[(int)EditorPrefabList.HABIT]._tableDataType;
            Dictionary<int, DBBaseTable> habits = Instance._tables[habitType];
            foreach(KeyValuePair<int, DBBaseTable> habit in habits)
            {
                DBHabit dbHabit = habit.Value as DBHabit;
                GameObject objHabit = GameObject.Instantiate(Instance._editorPrefabs[EditorPrefabList.HABIT]) as GameObject;
                objHabit.GetComponent<HabitAgent>().Build(dbHabit);
                objHabit.name = objHabit.name.Replace("(Clone)", "");
            }
        }


        public UnityEngine.Object GetEditorPrefab(EditorPrefabList key)
		{
			return _editorPrefabs [key];
		}

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
            _tables.Add(typeof(T),tableMap);
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

