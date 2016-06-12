using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PatternSystem
{
    abstract public class AttributeAgent : MonoBehaviour {

        protected ePatternList _attributeType;
        public ePatternList AttributeType { get { return _attributeType; } }

        protected static System.Type[] s_attributeTypes = {
                                                               typeof(ArrangeAgent),
                                                               typeof(CallAgent),
                                                               typeof(TimerAgent),
                                                               typeof(MoveAgent),
                                                               typeof(RotationAgent),
                                                               typeof(OrbitAgent),
                                                               typeof(ScaleAgent),
                                                           };


        protected int _id = -1;
        public int ID {get { return _id; }}

        public static List<AttributeAgent> CollectAttribute(Transform target)
		{
            List<AttributeAgent> attributes = new List<AttributeAgent> ();
            int cnt = target.childCount;
			for (int i = 0; i < cnt; ++i)
			{
                Transform t = target.GetChild(i);
                for(int j = 0; j < s_attributeTypes.Length; ++j)
                {
                    AttributeAgent attribute = t.GetComponent(s_attributeTypes[j]) as AttributeAgent;
                    if (attribute != null)
                    {
                        attributes.Add(attribute);
                        break;
                    }
                }
			}
            return attributes;
		}
        public abstract void Run(GameObject target);
        public abstract bool Save(DBAgent.MonoSQLiteManager dbManager, int parentID, string parentType, int sequence, bool isOverWrite = false);
        ///public abstract void Delete(DBAgent.MonoSQLiteManager dbManager);
        public abstract void Build(DBBaseTable dbData);
		public abstract Property GetProperty(GameObject target);
	}
}
