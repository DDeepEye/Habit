using UnityEngine;
using System.Collections;

namespace PatternSystem
{
    public class CallAgent : AttributeAgent
    {
        protected EditorPrefabList _attributeType;
        public EditorPrefabList AttributeType { get { return _attributeType; } }
        public string _sendMessage = "";
        public CallAgent()
        {
            _attributeType = EditorPrefabList.CALL;
        }

        public override void Run(GameObject target)
        {
        }

        public override void Save(DBAgent.MonoSQLiteManager dbManager, int parentID, string parentType, int sequence)
        {
            DBCall call = new DBCall();
            call.callName = _sendMessage;
            call.parentId = parentID;
            call.parentType = parentType;
            call.sequence = sequence;
            dbManager.InsertTable<DBCall>(ref call);
            dbManager.CommandQueries();
            call = dbManager.GetTableLastData<DBCall>();
            _id = call.id;
        }
    }
}
