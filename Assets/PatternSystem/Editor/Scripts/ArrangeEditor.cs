using UnityEngine;
using UnityEditor;
using PatternSystem;
using System.Collections;
using System.Collections.Generic;


namespace PatternSystem
{
    [CustomEditor(typeof(ArrangeAgent))]
	public class ArrangeEditor : AttributeEditor 
	{
        ArrangeAgent _arrange;
		void OnEnable () {
            _arrange = target as ArrangeAgent;
            _attribute = _arrange;
		}

		public override void OnInspectorGUI()
		{
            if (_arrange.transform.parent == null)
                return;

            if (_arrange.transform.parent.name != TrigerAgent.DBType && _arrange.transform.parent.name != "Arrange")
                return;
            
			EditorGUILayout.BeginVertical ();
			{
                _arrange._type = (ArrangeAgent.Type) EditorGUILayout.EnumPopup ("Arrange type", _arrange._type);
				_arrange._repeat = EditorGUILayout.IntField ("Repeat count",_arrange._repeat);
			}
			EditorGUILayout.EndVertical ();

			EditorGUILayout.BeginVertical ();
			{
				for(int i = (int)(ePatternList.ARRANGE); i < (int)(ePatternList.MAX); ++i)
				{
					string btnlabel = "Add --> ";
					btnlabel += ((ePatternList)i).ToString ();
					if(GUILayout.Button(btnlabel))
					{
						ePatternList key = (ePatternList)i;
						Object attribute = EditorResourcesPool.Instance.GetEditorPrefab (key);
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



