using UnityEngine;
using System.Collections;


namespace PatternSystem
{
    public class MoveAgent : PhysicalAgent
    {
        public const string DBType = "Move";
        public MoveAgent()
        {
            _attributeType = ePatternList.MOVE;
        }

        public override void Run(GameObject target)
        {
        }

        public override bool Save(DBAgent.MonoSQLiteManager dbManager, int parentID, string parentType, int sequence, bool isOverWrite = false)
        {
            DBPhysicalData physical = new DBPhysicalData();
            physical.id = ID;
            physical.isRelative = (int)_type;
            physical.parentId = parentID;
            physical.parentType = parentType;
            physical.physicalType = DBType;
            physical.sequence = sequence;
            physical.time = _time;
            physical.x = transform.localPosition.x;
            physical.y = transform.localPosition.y;
            physical.z = transform.localPosition.z;

            if (isOverWrite && ID != -1)
            {
                dbManager.UpdateTable<DBPhysicalData>(ref physical);
            }
            else
            {
                dbManager.InsertTable<DBPhysicalData>(ref physical);
            }

            if (!dbManager.CommandQueries())
                return false;
            if (!isOverWrite)
            {
                physical = dbManager.GetTableLastData<DBPhysicalData>();
                _id = physical.id;
            }
            return true;
        }

		public override Property GetProperty(GameObject target)
		{
			return new Move(target, _value, _time, _type);
		}


	}

}
