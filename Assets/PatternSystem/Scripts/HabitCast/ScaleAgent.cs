using UnityEngine;
using System.Collections;


namespace PatternSystem
{
    public class ScaleAgent : PhysicalAgent
    {
        public ScaleAgent()
        {
            _attributeType = EditorPrefabList.SCALE;
        }

        public override void Run(GameObject target)
        {
        }
        public override void Save(DBAgent.MonoSQLiteManager dbManager, int parentID, string parentType, int sequence)
        {
            DBPhysicalData physical = new DBPhysicalData();
            physical.isRelative = (int)_type;
            physical.parentId = parentID;
            physical.parentType = parentType;
            physical.physicalType = "Scale";
            physical.sequence = sequence;
            physical.time = _time;
            physical.x = transform.localPosition.x;
            physical.y = transform.localPosition.y;
            physical.z = transform.localPosition.z;
            dbManager.InsertTable<DBPhysicalData>(ref physical);
            dbManager.CommandQueries();
            physical = dbManager.GetTableLastData<DBPhysicalData>();
            _id = physical.id;
        }
    }
}
