using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace PatternSystem
{
	public class HabitAgent : MonoBehaviour {
        private static List<HabitAgent> s_Habits = new List<HabitAgent>();
        public static List<HabitAgent> Habits {get{return s_Habits;}}
        private EditorPrefabList _containerType;
        public EditorPrefabList ContainerType{get{return _containerType;}}

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
	}
}

