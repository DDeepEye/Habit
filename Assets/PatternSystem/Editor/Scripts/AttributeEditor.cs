using UnityEngine;
using UnityEditor;
using PatternSystem;
using System.Collections;
using System.Collections.Generic;

namespace PatternSystem
{
	[CustomEditor(typeof(Attribute))]
	public class AttributeEditor : Editor {

		protected Attribute _attribute;
		void OnEnable () {
			_attribute = target as Attribute;
		}

		public List<Attribute> CollectAttribute()
		{
			return _attribute.CollectAttribute();
		}
	}
}


