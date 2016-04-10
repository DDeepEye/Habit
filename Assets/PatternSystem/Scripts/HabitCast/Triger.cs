using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PatternSystem
{
	public class Triger : MonoBehaviour {

        private EditorPrefabList _containerType;
        public EditorPrefabList ContainerType { get { return _containerType; } }

		[SerializeField]
		private string _trigerName = "input triger key\t";
		public string TrigerName {get{ return _trigerName;} set{ _trigerName = value;}}

        public Triger()
        {
            _containerType = EditorPrefabList.TRIGER;
        }


		public List<Attribute> CollectAttribute()
		{
			List<Attribute> trigers = new List<Attribute> ();
			int cnt = transform.GetChildCount();
			for (int i = 0; i < cnt; ++i)
			{
				Transform t = transform.GetChild (i);
				Attribute triger = t.gameObject.GetComponent<Attribute> ();
				if (triger != null)
				{
					trigers.Add (triger);
				}				
			}
			return trigers;
		}
	}
}
