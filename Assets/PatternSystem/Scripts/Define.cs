using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DBAgent;

namespace PatternSystem
{
	public enum ePatternList
	{
		HABIT,
		TRIGER,
		ARRANGE,
		MOVE,
        SCALE,
        ROTATION,
        ORBIT,
        TIMER,
        CALL,
		MAX,
	}

    public abstract class DBBaseTable
    {
        [PRIMARY_KEY, AUTOINCREMENT, NOT_NULL, UNIQUE]
        public int      id;
    }

    public class DBHabit : DBBaseTable
	{
        public string comment;
		
	}

    public class DBTriger : DBBaseTable
	{
		public int 		habitId;
		public string 		trigerName;
	}

    public abstract class DBInterval : DBBaseTable
    {
        public string       parentType;
        public int          parentId;
    }

    public class DBArrange : DBInterval
	{
	
		public int			type;
		public int			repeat;
		public int		    sequence;
	}

    public class DBTimer : DBInterval
	{
	
		public float		time;
		public int			sequence;
	}

    public class DBCall : DBInterval
	{

		public string		callName;
		public int			sequence;
	}

    public class DBPhysicalData : DBInterval
	{
		public string		physicalType;
		public int			isRelative;
		public float		x;
		public float		y;
		public float		z;
		public float		time;
		public int			sequence;
	}

    public class DataClerk
    {
        private DataClerk(){}
        public static Dictionary<System.Type, Dictionary<int, DBBaseTable> > s_tables;
        public static Dictionary<ePatternList, UnityEngine.Object> s_editorPrefabs;

        public static Dictionary<int, DBBaseTable> GetTable(System.Type type)
        {
            return s_tables[type];
        }
        public static UnityEngine.Object GetPatternPrefab(ePatternList kind)
        {
            return s_editorPrefabs[kind];
        }


        public static readonly System.Type [] IntervalTypes = {typeof(DBArrange), typeof(DBTimer), typeof(DBCall), typeof(DBPhysicalData)};
        public static readonly Dictionary<string, ePatternList> Physicals = new Dictionary<string, ePatternList>()
        {
            {MoveAgent.DBType, ePatternList.MOVE},
            {OrbitAgent.DBType, ePatternList.ORBIT},
            {RotationAgent.DBType, ePatternList.ROTATION},
            {ScaleAgent.DBType, ePatternList.SCALE},
        };

        public static readonly Dictionary<string, System.Type> PhysicalComponents = new Dictionary<string, System.Type>()
        {
            {MoveAgent.DBType, typeof(MoveAgent)},
            {OrbitAgent.DBType, typeof(OrbitAgent)},
            {RotationAgent.DBType, typeof(RotationAgent)},
            {ScaleAgent.DBType,typeof(ScaleAgent)},
        };


        public static readonly Dictionary<System.Type, ePatternList> Intervals = new Dictionary<System.Type, ePatternList>()
        {
            {typeof(DBArrange), ePatternList.ARRANGE}, 
            {typeof(DBTimer), ePatternList.TIMER}, 
            {typeof(DBCall), ePatternList.CALL}
        };

        public static readonly Dictionary<System.Type, System.Type> IntervalComponents = new Dictionary<System.Type, System.Type>()
        {
            {typeof(DBArrange), typeof(ArrangeAgent)}, 
            {typeof(DBTimer), typeof(TimerAgent)}, 
            {typeof(DBCall), typeof(CallAgent)}
        };
    }
}
