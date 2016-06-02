﻿using UnityEngine;
using System.Collections;

namespace PatternSystem
{
    public class CallAgent : AttributeAgent
    {
        
        public string _sendMessage = "";
        public CallAgent()
        {
            _attributeType = ePatternList.CALL;
        }

        public override void Run(GameObject target)
        {
        }

        public override bool Save(DBAgent.MonoSQLiteManager dbManager, int parentID, string parentType, int sequence)
        {
            DBCall call = new DBCall();
            call.callName = _sendMessage;
            call.parentId = parentID;
            call.parentType = parentType;
            call.sequence = sequence;
            dbManager.InsertTable<DBCall>(ref call);
            if (!dbManager.CommandQueries())
                return false;

            call = dbManager.GetTableLastData<DBCall>();
            _id = call.id;
            return true;
        }

        public override void Build(DBBaseTable dbData)
        {
            DBCall dbCall = dbData as DBCall;
            _id = dbCall.id;
            _sendMessage = dbCall.callName;
        }

		public override Property GetProperty(GameObject target)
		{
			Caller p = new Caller (target);
			return p;
		}
    }
}
