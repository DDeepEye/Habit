using UnityEngine;
using System.Collections;


namespace PatternSystem
{
    public abstract class PhysicalAgent : AttributeAgent
    {
        public enum Type
        {
            RELATIVE,
            ABSULUTE,
        }

        public Vector3 _value = new Vector3();
        public float _time = 0.0f;
        public Type _type = Type.RELATIVE;

        public override void Run(GameObject target)
        {
        }

        //public abstract void Save(DBAgent.MonoSQLiteManager dbManager, int parentID, string parentType, int sequence);

    }
}

