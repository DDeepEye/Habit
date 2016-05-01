using UnityEngine;
using UnityEditor;
using PatternSystem;
using System.Collections;
using System.Collections.Generic;


namespace PatternSystem
{
	[CustomEditor(typeof(Arrange))]
	public class ArrangeEditor : AttributeEditor 
	{
		Arrange _arrange;
		void OnEnable () {
			_arrange = target as Arrange;
            _attribute = _arrange;
		}

		public override void OnInspectorGUI()
		{
			EditorGUILayout.BeginVertical ();
			{
				_arrange._type = (Arrange.Type) EditorGUILayout.EnumPopup ("Arrange type", _arrange._type);
				_arrange._repeat = EditorGUILayout.IntField ("Repeat count",_arrange._repeat);
			}
			EditorGUILayout.EndVertical ();

			EditorGUILayout.BeginVertical ();
			{
				for(int i = (int)(EditorPrefabList.ARRANGE); i < (int)(EditorPrefabList.MAX); ++i)
				{
					string btnlabel = "Add --> ";
					btnlabel += ((EditorPrefabList)i).ToString ();
					if(GUILayout.Button(btnlabel))
					{
						EditorPrefabList key = (EditorPrefabList)i;
						Object attribute = ResourcesPool.Instance.GetEditorPrefab (key);
						GameObject triger = PrefabUtility.InstantiatePrefab(attribute) as GameObject;
                        triger.transform.SetParent(_arrange.transform);
					}
				}
			}

			EditorGUILayout.EndVertical ();

			if (GUI.changed)
				EditorUtility.SetDirty(_attribute);

		}

	}

}



