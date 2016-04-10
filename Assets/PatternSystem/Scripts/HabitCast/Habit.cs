using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace PatternSystem
{
	public class Habit : MonoBehaviour {

        private EditorPrefabList _containerType;
        public EditorPrefabList ContainerType{get{return _containerType;}}
        public Habit()
        {
            _containerType = EditorPrefabList.HABIT;
        }

		public List<Triger> CollectTriger()
		{
			List<Triger> trigers = new List<Triger> ();
			int cnt = transform.GetChildCount();
			for (int i = 0; i < cnt; ++i)
			{
				Transform t = transform.GetChild (i);
				Triger triger = t.gameObject.GetComponent<Triger> ();
				if (triger != null)
				{
					trigers.Add (triger);
				}				
			}
			return trigers;

		}
	}
}

