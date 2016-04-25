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
		
	}

    public class DBTriger : DBBaseTable
	{
		public int 		habitId;
		public string 		tirigerName;
	}

    public class DBArrange : DBBaseTable
	{
		public int			parentType;
		public int			parentId;
		public int			type;
		public int			repeat;
		public float		sequence;
	}

    public class DBTimer : DBBaseTable
	{
		public string		parentType;
		public int			parentId;
		public int			isRelative;
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
