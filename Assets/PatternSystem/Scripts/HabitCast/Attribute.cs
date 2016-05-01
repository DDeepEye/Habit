using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PatternSystem
{
	abstract public class Attribute : MonoBehaviour {

        protected EditorPrefabList _attributeType;
        public EditorPrefabList AttributeType { get { return _attributeType; } }
        protected bool _isDone = false;
        public bool IsDone {get{ return _isDone;}}

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
        public abstract void Run(GameObject target);
        public virtual void Reset()
        {
            _isDone = false;
        }
	}
}
