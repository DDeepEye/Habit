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
        CHILD_CONTROL,
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

        public static Dictionary<int, DBBaseTable> GetTable(System.Type type)
        {
            return s_tables[type];
        }
    }
}
