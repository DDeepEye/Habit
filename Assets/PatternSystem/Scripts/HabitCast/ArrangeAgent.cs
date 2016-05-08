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
                if (!attributes[i].Save(dbManager, _id, DBType, i))
                    return false;

            }
            return true;
        }


        private readonly System.Type [] IntervalTypes = {typeof(DBArrange), typeof(DBTimer), typeof(DBCall), typeof(DBPhysicalData)};
        private readonly Dictionary<string, EditorPrefabList> Physicals = new Dictionary<string, EditorPrefabList>()
        {
            {MoveAgent.DBType, EditorPrefabList.MOVE},
            {OrbitAgent.DBType, EditorPrefabList.ORBIT},
            {RotationAgent.DBType, EditorPrefabList.ROTATION},
            {ScaleAgent.DBType, EditorPrefabList.SCALE},
        };

        private readonly Dictionary<string, System.Type> PhysicalComponents = new Dictionary<string, System.Type>()
        {
            {MoveAgent.DBType, typeof(MoveAgent)},
            {OrbitAgent.DBType, typeof(OrbitAgent)},
            {RotationAgent.DBType, typeof(RotationAgent)},
            {ScaleAgent.DBType,typeof(ScaleAgent)},
        };


        private readonly Dictionary<System.Type, EditorPrefabList> Intervals = new Dictionary<System.Type, EditorPrefabList>()
        {
            {typeof(DBArrange), EditorPrefabList.ARRANGE}, 
            {typeof(DBTimer), EditorPrefabList.TIMER}, 
            {typeof(DBCall), EditorPrefabList.CALL}
        };

        private readonly Dictionary<System.Type, System.Type> IntervalComponents = new Dictionary<System.Type, System.Type>()
        {
            {typeof(DBArrange), typeof(ArrangeAgent)}, 
            {typeof(DBTimer), typeof(TimerAgent)}, 
            {typeof(DBCall), typeof(CallAgent)}
        };

        public override void Build(DBBaseTable dbData)
        {
            DBArrange dbArrange = dbData as DBArrange;
            _type = dbArrange.sequence == 0 ? Type.SERIES : Type.PARALLEL;
            _id = dbArrange.id;
            _repeat = dbArrange.repeat;

            for (int i = 0; i < IntervalTypes.Length; ++i)
            {
                foreach (KeyValuePair<int, DBBaseTable> it in HabitAgent.s_tables[IntervalTypes[i]])
                {
                    DBInterval dbInterval = it.Value as DBInterval;
                    if (dbInterval.parentType == DBType && dbInterval.parentId == _id)
                    {
                        AttributeAgent aa = null;
                        if (IntervalTypes[i] == typeof(DBPhysicalData))
                        {
                            DBPhysicalData physicalData = dbInterval as DBPhysicalData;
                            EditorPrefabList type = Physicals[physicalData.physicalType];
                            GameObject physical = Instantiate(HabitAgent.s_editorPrefabs[type]) as GameObject;
                            physical.transform.SetParent(transform);
                            physical.name = physical.name.Replace("(Clone)", "");
                            aa = physical.GetComponent(PhysicalComponents[physicalData.physicalType]) as AttributeAgent;
                        }
                        else
                        {
                            EditorPrefabList type = Intervals[IntervalTypes[i]];
                            GameObject interval = Instantiate(HabitAgent.s_editorPrefabs[type]) as GameObject;
                            interval.transform.SetParent(transform);
                            interval.name = interval.name.Replace("(Clone)", "");
                            aa = interval.GetComponent(IntervalComponents[IntervalTypes[i]]) as AttributeAgent;
                        }
                        aa.Build(dbInterval);
                    }
                }
            }
        }
	}
}


