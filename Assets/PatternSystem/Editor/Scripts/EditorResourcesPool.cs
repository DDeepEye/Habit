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
            LoadTables();
		}

	}
}

