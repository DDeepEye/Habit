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

        public void Build(DBTriger dbTriger)
        {
            TrigerName = dbTriger.trigerName;
            _id = dbTriger.id;

            for (int i = 0; i < DataClerk.IntervalTypes.Length; ++i)
            {
                foreach (KeyValuePair<int, DBBaseTable> it in DataClerk.GetTable(DataClerk.IntervalTypes[i]))
                {
                    DBInterval dbInterval = it.Value as DBInterval;
                    if (dbInterval.parentType == DBType && dbInterval.parentId == ID)
                    {
                        AttributeAgent aa = null;
                        if (DataClerk.IntervalTypes[i] == typeof(DBPhysicalData))
                        {
                            DBPhysicalData physicalData = dbInterval as DBPhysicalData;
                            EditorPrefabList type = DataClerk.Physicals[physicalData.physicalType];
                            GameObject physical = Instantiate(DataClerk.GetPatternPrefab(type)) as GameObject;
                            physical.name = physical.name.Replace("(Clone)", "");
                            physical.transform.SetParent(transform);
                            aa = physical.GetComponent(DataClerk.PhysicalComponents[physicalData.physicalType]) as AttributeAgent;
                        }
                        else
                        {
                            EditorPrefabList type = DataClerk.Intervals[DataClerk.IntervalTypes[i]];
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
	}
}
