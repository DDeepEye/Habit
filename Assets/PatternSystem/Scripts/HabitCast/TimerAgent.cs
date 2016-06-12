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

        public override bool Save(DBAgent.MonoSQLiteManager dbManager, int parentID, string parentType, int sequence, bool isOverWrite = false)
        {
            DBTimer timer = new DBTimer();
            timer.id = ID;
            timer.parentId = parentID;
            timer.parentType = parentType;
            timer.sequence = sequence;
            timer.time = _time;
            if (isOverWrite && ID != -1)
            {
                dbManager.UpdateTable<DBTimer>(ref timer);
            }
            else
            {
                dbManager.InsertTable<DBTimer>(ref timer);
            }

            if (!dbManager.CommandQueries())
                return false;
            if (!isOverWrite)
            {
                timer = dbManager.GetTableLastData<DBTimer>();
                _id = timer.id;
            }
            return true;
        }
        public override void Build(DBBaseTable dbData)
        {
        }

		public override Property GetProperty(GameObject target)
		{
			return new Timer(target, _time);
		}
    }


}

