using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace PatternSystem
{
    
	public class HabitAgent : MonoBehaviour {
        private static List<HabitAgent> s_Habits = new List<HabitAgent>();
        public static GameObject s_listManager;
        public static List<HabitAgent> Habits {get{return s_Habits;}}

        private EditorPrefabList _containerType;
        public EditorPrefabList ContainerType{get{return _containerType;}}

        public string _activeTriger = "";

        private int _id = -1;
        public int ID { set{ _id = value;} get{return _id;}}
        public string _comment = "comment";
        public HabitAgent()
        {
            s_Habits.Add(this);
            _containerType = EditorPrefabList.HABIT;
            Debug.Log("new HabitAgent current count " + s_Habits.Count);
        }
        ~HabitAgent()
        {
            s_Habits.Remove(this);
            Debug.Log("Remove HabitAgent current count " + s_Habits.Count);
        }

        void Start()
        {
            gameObject.hideFlags = HideFlags.HideInHierarchy;
        }

        public List<TrigerAgent> CollectTriger()
		{
            List<TrigerAgent> trigers = new List<TrigerAgent> ();
            int cnt = transform.childCount;
			for (int i = 0; i < cnt; ++i)
			{
				Transform t = transform.GetChild (i);
                TrigerAgent triger = t.gameObject.GetComponent<TrigerAgent>();
				if (triger != null)
				{
					trigers.Add (triger);
				}				
			}
			return trigers;
		}

        public bool Save(DBAgent.MonoSQLiteManager dbManager)
        {
            DBHabit habit = new DBHabit();
            habit.comment = _comment;
            dbManager.InsertTable<DBHabit>(ref habit);
            if (!dbManager.CommandQueries())
                return false;
            habit = dbManager.GetTableLastData<DBHabit>();
            _id = habit.id;
            List<TrigerAgent> trigers = CollectTriger();
            foreach (TrigerAgent ta in trigers)
            {
                if (!ta.Save(dbManager, _id))
                    return false;
            }
            return true;
        }

        public void Build(DBHabit dbHabit)
        {
            _comment = dbHabit.comment;
            _id = dbHabit.id;

            transform.SetParent(s_listManager.transform);

            Dictionary<int, DBBaseTable> trigers = DataClerk.GetTable(typeof(DBTriger));
            foreach (KeyValuePair<int, DBBaseTable> triger in trigers)
            {
                DBTriger dbTriger = triger.Value as DBTriger;
                if (dbHabit.id == dbTriger.habitId)
                {
                        GameObject trigerObj = GameObject.Instantiate(DataClerk.GetPatternPrefab(EditorPrefabList.TRIGER)) as GameObject;
                    trigerObj.name = trigerObj.name.Replace("(Clone)", "");
                    trigerObj.transform.SetParent(transform);
                    trigerObj.GetComponent<TrigerAgent>().Build(dbTriger);
                }
            }
        }
	}
}

