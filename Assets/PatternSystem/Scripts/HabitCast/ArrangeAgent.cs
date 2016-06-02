using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PatternSystem
{
    public class ArrangeAgent : AttributeAgent 
    {
        public const string DBType = "Arrange";


        public ArrangeAgent()
        {
            _attributeType = ePatternList.ARRANGE;
        }


		public Arrange.ArrangeType _type = Arrange.ArrangeType.SERIES;
		public int _repeat = 1;

        public override void Run(GameObject target)
        {
        }

        public override bool Save(DBAgent.MonoSQLiteManager dbManager, int parentID, string parentType, int sequence)
        {
            DBArrange arrange = new DBArrange();
            arrange.parentId = parentID;
            arrange.parentType = parentType;
            arrange.repeat = _repeat;
            arrange.sequence = sequence;
            arrange.type = (int)_type;
            dbManager.InsertTable<DBArrange>(ref arrange);
            if (!dbManager.CommandQueries())
                return false;

            arrange = dbManager.GetTableLastData<DBArrange>();
            _id = arrange.id;
            List<AttributeAgent> attributes = AttributeAgent.CollectAttribute(transform);
            for (int i = 0; i < attributes.Count; ++i)
            {
                if (!attributes[i].Save(dbManager, _id, DBType, i))
                    return false;

            }
            return true;
        }


        
        public override void Build(DBBaseTable dbData)
        {
            DBArrange dbArrange = dbData as DBArrange;
			_type = dbArrange.sequence == 0 ? Arrange.ArrangeType.SERIES : Arrange.ArrangeType.PARALLEL;
            _id = dbArrange.id;
            _repeat = dbArrange.repeat;

            for (int i = 0; i < DataClerk.IntervalTypes.Length; ++i)
            {
                foreach (KeyValuePair<int, DBBaseTable> it in DataClerk.GetTable(DataClerk.IntervalTypes[i]))
                {
                    DBInterval dbInterval = it.Value as DBInterval;
                    if (dbInterval.parentType == DBType && dbInterval.parentId == _id)
                    {
                        AttributeAgent aa = null;
                        if (DataClerk.IntervalTypes[i] == typeof(DBPhysicalData))
                        {
                            DBPhysicalData physicalData = dbInterval as DBPhysicalData;
                            ePatternList type = DataClerk.Physicals[physicalData.physicalType];
                            GameObject physical = Instantiate(DataClerk.GetPatternPrefab(type)) as GameObject;
                            physical.transform.SetParent(transform);
                            physical.name = physical.name.Replace("(Clone)", "");
                            aa = physical.GetComponent(DataClerk.PhysicalComponents[physicalData.physicalType]) as AttributeAgent;
                        }
                        else
                        {
                            ePatternList type = DataClerk.Intervals[DataClerk.IntervalTypes[i]];
                            GameObject interval = Instantiate(DataClerk.GetPatternPrefab(type)) as GameObject;
                            interval.transform.SetParent(transform);
                            interval.name = interval.name.Replace("(Clone)", "");
                            aa = interval.GetComponent(DataClerk.IntervalComponents[DataClerk.IntervalTypes[i]]) as AttributeAgent;
                        }
                        aa.Build(dbInterval);
                    }
                }
            }
        }

		public override Property GetProperty(GameObject target)
		{
			List<Property> properties = new List<Property> ();
			List<AttributeAgent> attributes = AttributeAgent.CollectAttribute(gameObject.transform);
			foreach (AttributeAgent att in attributes)
			{
				properties.Add(att.GetProperty (target));
			}

			Arrange p = new Arrange (target, _type, properties);

			return p;
		}
	}
}


