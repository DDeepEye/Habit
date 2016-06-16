using UnityEngine;
using UnityEditor;
using PatternSystem;
using System.Collections;
using System.Collections.Generic;

namespace PatternSystem
{
    [CustomEditor(typeof(TrigerAgent))]
	public class TrigerEditor : AttributeEditor {

        TrigerAgent _triger;
		void OnEnable () {

            _triger = target as TrigerAgent;

		}

		public override void OnInspectorGUI()
		{
            if (_triger.transform.parent == null)
            {
                GUILayout.Box("absolute Triger to Habit child");
                return;
            }

            if (_triger.transform.parent.name != "Habit")
            {
                GUILayout.Box("absolute Triger to Habit child");
                return;
            }

			EditorGUILayout.BeginHorizontal();
			{
				_triger.TrigerName	 = EditorGUILayout.TextField("Key Name", _triger.TrigerName);
			}
			EditorGUILayout.EndHorizontal ();
            for(int i = 0; i < EditorResourcesPool.Instance.GetEditorPrefabCount(); ++i)
            {
                string btnlabel = "Add --> ";
                btnlabel += EditorResourcesPool.Instance.GetEditorPrefab(i).name;
				if(GUILayout.Button(btnlabel))
				{
                    Object attribute = EditorResourcesPool.Instance.GetEditorPrefab(i);
					GameObject triger = PrefabUtility.InstantiatePrefab(attribute) as GameObject;
					triger.transform.SetParent (_triger.transform);
				}
			}

			if (GUI.changed)
				EditorUtility.SetDirty(_triger);

		}
	}	
}


