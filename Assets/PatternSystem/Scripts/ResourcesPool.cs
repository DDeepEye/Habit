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

		void Init()
		{
			for (int i = 0; i < _editorPrefabPaths.Length; ++i) 
			{
                Object o = AssetDatabase.LoadAssetAtPath(_editorPrefabPaths[i]._path, typeof(Object));
				_editorPrefabs.Add (_editorPrefabPaths [i]._key, o);
			}
		}




		public Object GetEditorPrefab(EditorPrefabList key)
		{
			return _editorPrefabs [key];
		}
	}
}

