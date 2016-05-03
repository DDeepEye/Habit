using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PatternSystem
{
    public class TrigerAgent : MonoBehaviour {

        private EditorPrefabList _containerType;
        public EditorPrefabList ContainerType { get { return _containerType; } }

		[SerializeField]
		private string _trigerName = "input triger key";
		public string TrigerName {get{ return _trigerName;} set{ _trigerName = value;}}

        private int _id = -1;

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

        public void Save(DBAgent.MonoSQLiteManager dbManager, int habitDbId)
        {
            DBTriger triger = new DBTriger();
            triger.habitId = habitDbId;
            triger.trigerName = _trigerName;
            dbManager.InsertTable<DBTriger>(ref triger);
            dbManager.CommandQueries();
            triger = dbManager.GetTableLastData<DBTriger>();
            _id = triger.id;
            List<AttributeAgent> attributes = CollectAttribute();
            foreach (AttributeAgent att in attributes)
            {
                att.Save(dbManager, _id, "Triger", 0);
            }
        }
	}
}
