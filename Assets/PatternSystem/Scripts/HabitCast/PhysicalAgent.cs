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
        private int _sequence = 0;
        public int Sequence {get{ return _sequence; } set{ _sequence = value;}}

        public override void Build(DBBaseTable dbData)
        {
            DBPhysicalData dbPhysic= dbData as DBPhysicalData;
            _id = dbPhysic.id;
            _type = dbPhysic.isRelative == 0 ? Type.RELATIVE : Type.ABSULUTE; 
            _time = dbPhysic.time;
            _sequence = dbPhysic.sequence;
            _value.x = dbPhysic.x;
            _value.y = dbPhysic.y;
            _value.z = dbPhysic.z;
        }
    }


}

