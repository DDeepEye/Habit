using UnityEngine;
using UnityEditor;
using PatternSystem;
using System.Collections;
using System.Collections.Generic;

namespace PatternSystem
{
    [CustomEditor(typeof(HabitAgent))]
	public class HabitEditor : Editor {

        HabitAgent _habit;


		void OnEnable () {

            _habit = target as HabitAgent;
		}

		public override void OnInspectorGUI()
		{
			EditorGUILayout.BeginVertical ();
			{
                if (_habit.transform.parent != null)
                {
                    _habit._comment = EditorGUILayout.TextField("Habit Comment", _habit._comment);
                    if (GUILayout.Button("Add Triger"))
                    {
                        PatternSystem.EditorPrefabList key = PatternSystem.EditorPrefabList.TRIGER;
                        Object habit = PatternSystem.ResourcesPool.Instance.GetEditorPrefab(key);
                        GameObject triger = PrefabUtility.InstantiatePrefab(habit) as GameObject;
                        triger.name = triger.name.Replace("(clone)", "");
                        triger.transform.SetParent(_habit.transform);
                    }
                }
                else
                {
                    GUILayout.Box("absolute Habit to GameObject child");
                }
			}
			EditorGUILayout.EndVertical ();

			if (GUI.changed)
				EditorUtility.SetDirty(target);


		}

	}
}


