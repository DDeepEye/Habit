using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PatternSystem
{
    public class ArrangeAgent : AttributeAgent 
    {
        public ArrangeAgent()
        {
            _attributeType = EditorPrefabList.ARRANGE;
        }
		public enum Type
		{
			SERIES,
			PARALLEL,
		}

		public Type _type = Type.SERIES;
		public int _repeat = 1;

        public override void Run(GameObject target)
        {
        }

        public override void Save(DBAgent.MonoSQLiteManager dbManager, int parentID, string parentType, int sequence)
        {
            DBArrange arrange = new DBArrange();
            arrange.parentId = parentID;
            arrange.parentType = parentType;
            arrange.repeat = _repeat;
            arrange.sequence = sequence;
            arrange.type = (int)_type;
            dbManager.InsertTable<DBArrange>(ref arrange);
            dbManager.CommandQueries();
            arrange = dbManager.GetTableLastData<DBArrange>();
            _id = arrange.id;
            List<AttributeAgent> attributes = AttributeAgent.CollectAttribute(transform);
            for (int i = 0; i < attributes.Count; ++i)
            {
                attributes[i].Save(dbManager, _id, "Arrange", i);
            }
        }

	}
}


