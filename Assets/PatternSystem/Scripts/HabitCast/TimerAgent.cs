using UnityEngine;
using System.Collections;

namespace PatternSystem
{
    public class TimerAgent : AttributeAgent
    {
        public float _time = 0.0f;
        public override void Run(GameObject target)
        {
        }

        public override void Save(DBAgent.MonoSQLiteManager dbManager, int parentID, string parentType, int sequence)
        {
            DBTimer timer = new DBTimer();
            timer.parentId = parentID;
            timer.parentType = parentType;
            timer.sequence = sequence;
            timer.time = _time;
            dbManager.InsertTable<DBTimer>(ref timer);
            dbManager.CommandQueries();
            timer = dbManager.GetTableLastData<DBTimer>();
            _id = timer.id;
        }
    }


}

