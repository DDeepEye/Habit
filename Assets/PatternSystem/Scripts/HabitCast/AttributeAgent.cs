using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PatternSystem
{
    abstract public class AttributeAgent : MonoBehaviour {

        protected ePatternList _attributeType;
        public ePatternList AttributeType { get { return _attributeType; } }


        protected int _id = -1;

        public static List<AttributeAgent> CollectAttribute(Transform target)
		{
            List<AttributeAgent> attributes = new List<AttributeAgent> ();
            int cnt = target.childCount;
			for (int i = 0; i < cnt; ++i)
			{
                Transform t = target.GetChild (i);
                AttributeAgent attribute = t.GetComponent<ArrangeAgent> ();
                if (attribute != null)
				{
                    attributes.Add (attribute);
                    continue;
				}

                attribute = t.GetComponent<MoveAgent> ();
                if (attribute != null)
                {
                    attributes.Add (attribute);
                    continue;
                }

                attribute = t.GetComponent<ScaleAgent> ();
                if (attribute != null)
                {
                    attributes.Add (attribute);
                    continue;
                }

                attribute = t.GetComponent<TimerAgent> ();
                if (attribute != null)
                {
                    attributes.Add (attribute);
                    continue;
                }

                attribute = t.GetComponent<CallAgent> ();
                if (attribute != null)
                {
                    attributes.Add (attribute);
                    continue;
                }

                attribute = t.GetComponent<OrbitAgent> ();
                if (attribute != null)
                {
                    attributes.Add (attribute);
                    continue;
                }

                attribute = t.GetComponent<RotationAgent> ();
                if (attribute != null)
                {
                    attributes.Add (attribute);
                    continue;
                }
			}
            return attributes;
		}
        public abstract void Run(GameObject target);
        public abstract bool Save(DBAgent.MonoSQLiteManager dbManager, int parentID, string parentType, int sequence);
        public abstract void Build(DBBaseTable dbData);
	}
}
