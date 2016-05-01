using UnityEngine;
using System.Collections;

namespace PatternSystem
{
    public class Call : Attribute
    {
        protected EditorPrefabList _attributeType;
        public EditorPrefabList AttributeType { get { return _attributeType; } }
        public string _sendMessage = "";
        public Call()
        {
            _attributeType = EditorPrefabList.CALL;
        }

        public override void Run(GameObject target)
        {
        }
    }
}
