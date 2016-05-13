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

        int _trigerSelected = 0;


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
                        PatternSystem.ePatternList key = PatternSystem.ePatternList.TRIGER;
                        Object habit = PatternSystem.EditorResourcesPool.Instance.GetEditorPrefab(key);
                        GameObject triger = PrefabUtility.InstantiatePrefab(habit) as GameObject;
                        triger.name = triger.name.Replace("(clone)", "");
                        triger.transform.SetParent(_habit.transform);
                    }
                    List<TrigerAgent> trigers = _habit.CollectTriger();
                    if(trigers.Count > 0)
                    {
                        List<string> trigerNames = new List<string>();
                        HashSet<string> overlap = new HashSet<string>();
                        foreach(TrigerAgent triger in trigers)
                        {
                            trigerNames.Add(triger.TrigerName);
                            if (overlap.Contains(triger.TrigerName))
                                Debug.Log("Overlap Triger Name !! -> " + triger.TrigerName);
                            else
                                overlap.Add(triger.TrigerName);
                            
                        }                       

                        EditorGUILayout.BeginHorizontal();
                        _trigerSelected = EditorGUILayout.Popup("Choice Active Triger", _trigerSelected, trigerNames.ToArray());
                        EditorGUILayout.EndHorizontal();
                        _habit._activeTriger = trigerNames[_trigerSelected];
                    }
                    else
                    {
                        GUILayout.TextArea("Add Triger");
                    }
                    
                    ///_trigerSelected = GUILayout.SelectionGrid(_trigerSelected,)

                    
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


