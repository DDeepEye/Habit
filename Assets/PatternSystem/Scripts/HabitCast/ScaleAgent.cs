﻿using UnityEngine;
using System.Collections;


namespace PatternSystem
{
    public class ScaleAgent : PhysicalAgent
    {
        public const string DBType = "Scale";
        public ScaleAgent()
        {
            _attributeType = ePatternList.SCALE;
        }

        public override void Run(GameObject target)
        {
        }
        public override bool Save(DBAgent.MonoSQLiteManager dbManager, int parentID, string parentType, int sequence)
        {
            DBPhysicalData physical = new DBPhysicalData();
            physical.isRelative = (int)_type;
            physical.parentId = parentID;
            physical.parentType = parentType;
            physical.physicalType = DBType;
            physical.sequence = sequence;
            physical.time = _time;
            physical.x = transform.localPosition.x;
            physical.y = transform.localPosition.y;
            physical.z = transform.localPosition.z;
            dbManager.InsertTable<DBPhysicalData>(ref physical);
            if (dbManager.CommandQueries())
                return false;
            physical = dbManager.GetTableLastData<DBPhysicalData>();
            _id = physical.id;
            return true;
        }
        public override void Build(DBBaseTable dbData)
        {
        }

		public override Property GetProperty(GameObject target)
		{
			return new Scale(target, _value, _time, _type);
		}
    }
}
