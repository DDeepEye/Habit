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
			List<Attribute> attributes = new List<Attribute> ();
			int cnt = transform.GetChildCount();
			for (int i = 0; i < cnt; ++i)
			{
				Transform t = transform.GetChild (i);
				Attribute attribute = t.gameObject.GetComponent<Attribute> ();
                if (attribute != null)
				{
                    attributes.Add (attribute);
				}				
			}
            return attributes;
		}

        public void Run(GameObject target)
        {
        }
	}
}
