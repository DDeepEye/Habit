using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace PatternSystem
{   
	public class HabitAgent : MonoBehaviour {
        private static List<HabitAgent> s_Habits = new List<HabitAgent>();
        public static GameObject s_listManager;
        public static List<HabitAgent> Habits {get{return s_Habits;}}

        private ePatternList _containerType;
        public ePatternList ContainerType { get { return _containerType; } }

        public string _activeTriger = "";

        private int _id = -1;
        public int ID { set{ _id = value;} get{return _id;}}
        public string _comment = "comment";
        public HabitAgent()
        {
            s_Habits.Add(this);
            _containerType = ePatternList.HABIT;
            Debug.Log("new HabitAgent current count " + s_Habits.Count);
        }
        ~HabitAgent()
        {
            s_Habits.Remove(this);
            Debug.Log("Remove HabitAgent current count " + s_Habits.Count);
        }

        void Start()
        {
			if (transform.parent == null)
				return;

			if (transform.parent.gameObject.tag != "HabitTest")
				return;

            gameObject.hideFlags = HideFlags.HideInHierarchy;
			List<TrigerAgent> trigers= CollectTriger ();

			GameObject testTarget = transform.parent.gameObject;
			Habit habit = testTarget.GetComponent<Habit>();

			foreach (TrigerAgent triger in trigers)
			{
				habit.AddTriger(triger.GetTriger(transform.parent.gameObject));
			}

			habit.Play (_activeTriger);
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

        public bool Save(DBAgent.MonoSQLiteManager dbManager, bool isOverWrite = false)
        {
            DBHabit habit = new DBHabit();
            habit.comment = _comment;
            habit.id = ID;
            if (isOverWrite && ID != -1)
            {
                dbManager.UpdateTable<DBHabit>(ref habit);
            }
            else
            {
                dbManager.InsertTable<DBHabit>(ref habit);
            }

            if (!dbManager.CommandQueries())
                return false;
            if (!isOverWrite)
            {
                habit = dbManager.GetTableLastData<DBHabit>();
                _id = habit.id;
            }
            List<TrigerAgent> trigers = CollectTriger();
            foreach (TrigerAgent ta in trigers)
            {
                if (!ta.Save(dbManager, _id, isOverWrite))
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
                    GameObject trigerObj = GameObject.Instantiate(DataClerk.GetPatternPrefab(ePatternList.TRIGER)) as GameObject;
                    trigerObj.name = trigerObj.name.Replace("(Clone)", "");
                    trigerObj.transform.SetParent(transform);
                    trigerObj.GetComponent<TrigerAgent>().Build(dbTriger);
                }
            }
        }

        void Delete(DBAgent.MonoSQLiteManager dbManager)
        {
            
        }
	}
}

