using UnityEngine;
using UnityEditor;
using PatternSystem;
using System.Collections;
using System.Collections.Generic;

namespace PatternSystem
{
	[CustomEditor(typeof(Triger))]
	public class TrigerEditor : AttributeEditor {

		Triger _triger;
		void OnEnable () {

			_triger = target as Triger;

		}

		public override void OnInspectorGUI()
		{
			EditorGUILayout.BeginHorizontal();
			{
				_triger.TrigerName	 = EditorGUILayout.TextField("Key Name", _triger.TrigerName);
			}
			EditorGUILayout.EndHorizontal ();
			for(int i = (int)(EditorPrefabList.ARRANGE); i < (int)(EditorPrefabList.MAX); ++i)
			{
				string btnlabel = "Add --> ";
				btnlabel += ((EditorPrefabList)i).ToString ();
				if(GUILayout.Button(btnlabel))
				{
					EditorPrefabList key = (EditorPrefabList)i;
					Object attribute = ResourcesPool.Instance.GetEditorPrefab (key);
					GameObject triger = PrefabUtility.InstantiatePrefab(attribute) as GameObject;
					triger.transform.SetParent (_triger.transform);
				}
			}

			if (GUI.changed)
				EditorUtility.SetDirty(_triger);

		}
	}	
}


