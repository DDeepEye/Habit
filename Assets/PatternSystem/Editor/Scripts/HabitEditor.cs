using UnityEngine;
using UnityEditor;
using PatternSystem;
using System.Collections;
using System.Collections.Generic;

namespace PatternSystem
{
	[CustomEditor(typeof(Habit))]
	public class HabitEditor : Editor {

		Habit _habit;


		void OnEnable () {

			_habit = target as Habit;


		}

		public override void OnInspectorGUI()
		{
			EditorGUILayout.BeginVertical ();
			{
				if(GUILayout.Button("Add Triger"))
				{
					PatternSystem.EditorPrefabList key = PatternSystem.EditorPrefabList.TRIGER;
					Object habit = PatternSystem.ResourcesPool.Instance.GetEditorPrefab (key);
					GameObject triger = PrefabUtility.InstantiatePrefab(habit) as GameObject;
					triger.transform.SetParent (_habit.transform);
				}
			}
			EditorGUILayout.EndVertical ();

			if (GUI.changed)
				EditorUtility.SetDirty(target);


		}

		public List<Triger> CollectTriger()
		{
			return _habit.CollectTriger();
		}
	}
}


