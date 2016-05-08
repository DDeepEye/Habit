using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PatternSystem
{
    public class TrigerAgent : MonoBehaviour {

        private EditorPrefabList _containerType;
        public EditorPrefabList ContainerType { get { return _containerType; } }


        public const string DBType = "Triger";

		[SerializeField]
		private string _trigerName = "input triger key";
		public string TrigerName {get{ return _trigerName;} set{ _trigerName = value;}}

        private int _id = -1;
        public int ID{get{ return _id;} set{ _id = value;}}

        public TrigerAgent()
        {
            _containerType = EditorPrefabList.TRIGER;
        }


        public List<AttributeAgent> CollectAttribute()
        {
            return AttributeAgent.CollectAttribute(transform);
		}

        public void Run(GameObject target)
        {
            
        }

        public bool Save(DBAgent.MonoSQLiteManager dbManager, int habitDbId)
        {
            DBTriger triger = new DBTriger();
            triger.habitId = habitDbId;
            triger.trigerName = _trigerName;
            dbManager.InsertTable<DBTriger>(ref triger);
            if (dbManager.CommandQueries())
                return false;
            triger = dbManager.GetTableLastData<DBTriger>();
            _id = triger.id;
            List<AttributeAgent> attributes = CollectAttribute();
            foreach (AttributeAgent att in attributes)
            {
                if (att.Save(dbManager, _id, DBType, 0))
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

        public void Build(DBTriger dbTriger)
        {
            TrigerName = dbTriger.trigerName;
            _id = dbTriger.id;

            for (int i = 0; i < IntervalTypes.Length; ++i)
            {
                foreach (KeyValuePair<int, DBBaseTable> it in HabitAgent.s_tables[IntervalTypes[i]])
                {
                    DBInterval dbInterval = it.Value as DBInterval;
                    if (dbInterval.parentType == DBType && dbInterval.parentId == ID)
                    {
                        AttributeAgent aa = null;
                        if (IntervalTypes[i] == typeof(DBPhysicalData))
                        {
                            DBPhysicalData physicalData = dbInterval as DBPhysicalData;
                            EditorPrefabList type = Physicals[physicalData.physicalType];
                            GameObject physical = Instantiate(HabitAgent.s_editorPrefabs[type]) as GameObject;
                            physical.name = physical.name.Replace("(Clone)", "");
                            physical.transform.SetParent(transform);
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
