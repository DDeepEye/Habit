﻿using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using DBAgent;

namespace PatternSystem
{	public class EditorResourcesPool : ResourcesPool
	{        
		private static EditorResourcesPool s_instance;
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
		public static EditorResourcesPool Instance 
		{
			get
			{
				if (s_instance == null) 
				{
					s_instance = new EditorResourcesPool ();
					s_instance.Init ();
				}
				return s_instance;
			}
		}

		public struct EditorPrefabInfo
		{
			public EditorPrefabInfo(ePatternList key, string path, System.Type tableDataType)
			{
				_key = key;
                _path = path;
				_tableDataType = tableDataType;
			}
			public ePatternList 	_key;
			public string			    _path;
			public System.Type			_tableDataType;
		}

		EditorPrefabInfo [] _editorPrefabPaths = new EditorPrefabInfo[]{
			new EditorPrefabInfo(
                ePatternList.HABIT, "Assets/PatternSystem/Editor/EditorPrefabs/Habit.prefab", typeof(DBHabit)),
            new EditorPrefabInfo(ePatternList.TRIGER, "Assets/PatternSystem/Editor/EditorPrefabs/Triger.prefab", typeof(DBTriger)),
            new EditorPrefabInfo(ePatternList.ARRANGE, "Assets/PatternSystem/Editor/EditorPrefabs/Arrange.prefab", typeof(DBArrange)),
            new EditorPrefabInfo(ePatternList.MOVE, "Assets/PatternSystem/Editor/EditorPrefabs/Move.prefab", typeof(DBPhysicalData)),
            new EditorPrefabInfo(ePatternList.SCALE, "Assets/PatternSystem/Editor/EditorPrefabs/Scale.prefab", typeof(DBPhysicalData)),
            new EditorPrefabInfo(ePatternList.ROTATION, "Assets/PatternSystem/Editor/EditorPrefabs/Rotation.prefab", typeof(DBPhysicalData)),
            new EditorPrefabInfo(ePatternList.ORBIT, "Assets/PatternSystem/Editor/EditorPrefabs/Orbit.prefab", typeof(DBPhysicalData)),
            new EditorPrefabInfo(ePatternList.TIMER, "Assets/PatternSystem/Editor/EditorPrefabs/Timer.prefab", typeof(DBTimer)),
            new EditorPrefabInfo(ePatternList.CALL, "Assets/PatternSystem/Editor/EditorPrefabs/Call.prefab", typeof(DBCall)),
		};
        Dictionary<ePatternList,UnityEngine.Object> _editorPrefabs = new Dictionary<ePatternList, UnityEngine.Object>();        

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
        static private void SaveCurrentPatterns()
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

        static public void SavePattern(HabitAgent ha, bool isOverWrite = false)
        {
            MonoSQLiteManager dbManager = new MonoSQLiteManager(PATH + DBFILENAME);
            string backupFileName = PATH + DateTime.Now.ToString("yyyy_mm_dd")+ ".db";
            FileInfo file = new FileInfo(PATH + DBFILENAME);
            if(file.Exists)
            {
                file.CopyTo(backupFileName, true);
            }

            if(ha.transform.parent != null)
            {
                if(!ha.Save(dbManager, isOverWrite))
                {
                    FileInfo bfile = new FileInfo(backupFileName);
                    if(bfile.Exists)
                    {
                        bfile.CopyTo(PATH + DBFILENAME);
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

            Type habitType = Instance._editorPrefabPaths[(int)ePatternList.HABIT]._tableDataType;
            Dictionary<int, DBBaseTable> habits = Instance._tables[habitType];
            foreach(KeyValuePair<int, DBBaseTable> habit in habits)
            {
                DBHabit dbHabit = habit.Value as DBHabit;
                GameObject objHabit = GameObject.Instantiate(Instance._editorPrefabs[ePatternList.HABIT]) as GameObject;
                objHabit.GetComponent<HabitAgent>().Build(dbHabit);
                objHabit.name = objHabit.name.Replace("(Clone)", "");
            }
        }


        public UnityEngine.Object GetEditorPrefab(ePatternList key)
		{
			return _editorPrefabs [key];
		}        
	}
}

