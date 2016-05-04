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
                if (!attributes[i].Save(dbManager, _id, "Arrange", i))
                    return false;

            }
            return true;
        }

	}
}


