using UnityEngine;
using System.Collections;
using DBAgent;

namespace PatternSystem
{
	public enum EditorPrefabList
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

    public class DBArrange : DBBaseTable
	{
		public string		parentType;
		public int			parentId;
		public int			type;
		public int			repeat;
		public int		    sequence;
	}

    public class DBTimer : DBBaseTable
	{
		public string		parentType;
		public int			parentId;
		public float		time;
		public int			sequence;
	}

    public class DBCall : DBBaseTable
	{
		public string		parentType;
		public int			parentId;
		public string		callName;
		public int			sequence;
	}

    public class DBPhysicalData : DBBaseTable
	{
		public string		parentType;
		public int			parentId;
		public string		physicalType;
		public int			isRelative;
		public float		x;
		public float		y;
		public float		z;
		public float		time;
		public int			sequence;
	}
}
