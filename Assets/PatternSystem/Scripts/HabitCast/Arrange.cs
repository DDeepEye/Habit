using UnityEngine;
using System.Collections;

namespace PatternSystem
{
	public class Arrange : Attribute 
    {
        public Arrange()
        {
            _attributeType = EditorPrefabList.ARRANGE;
        }
		public enum Type
		{
			SERIES,
			PARALLEL,
		}

		public Type _type = Type.SERIES;
		public int _repeat = 1;

        public override void Run(GameObject target)
        {
        }


	}
}


