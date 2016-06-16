using UnityEngine;
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
        List<UnityEngine.Object> _editorPrefabs = new List<UnityEngine.Object>();

		void Init()
		{
            
            string[] GUIDs = AssetDatabase.FindAssets("", new string[] {"Assets/PatternSystem/Editor/EditorPrefabs"});

            for (int index = 0; index < GUIDs.Length; index++)
            {
                string guid = GUIDs[index];
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object)) as UnityEngine.Object;
                _editorPrefabs.Add(asset);
            }

            DataClerk.s_editorPrefabs = _editorPrefabs;
            LoadTables();
		}

        public int GetEditorPrefabCount()
        {
            return _editorPrefabs.Count;
        }

        public UnityEngine.Object GetEditorPrefab(int i)
		{
			return _editorPrefabs[i];
		}

        public UnityEngine.Object GetPatternPrefab(string name)
        {
            
            for (int i = 0; i < _editorPrefabs.Count; ++i)
            {
                if (_editorPrefabs[i].name == name)
                {
                    return _editorPrefabs[i];
                }
            }
            return null;
        }
	}
}

