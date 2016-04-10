using UnityEngine;
using System.Collections;


namespace PatternSystem
{
    public class Physical : Attribute
    {
        public enum Type
        {
            RELATIVE,
            ABSULUTE,
        }

        public Vector3 _value = new Vector3();
        public float _time = 0.0f;
        public Type _type = Type.RELATIVE;
    }
}

