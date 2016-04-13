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

	public struct DBHabit
	{
		[PRIMARY_KEY, AUTOINCREMENT, NOT_NULL, UNIQUE]
		public int 		id;
	}

	public struct DBTriger
	{
		[PRIMARY_KEY, AUTOINCREMENT, NOT_NULL, UNIQUE]
		public int 		id;
		public int 		habitId;
		public string 		tirigerName;
	}

	public struct DBArrange
	{
		[PRIMARY_KEY, AUTOINCREMENT, NOT_NULL, UNIQUE]
		public int			id;
		public int			parentType;
		public int			parentId;
		public int			type;
		public int			repeat;
		public float			sequence;
	}

	public struct DBTimer
	{
		[PRIMARY_KEY, AUTOINCREMENT, NOT_NULL, UNIQUE]
		public int			id;
		public string		parentType;
		public int			parentId;
		public int			isRelative;
		public float		time;
		public int			sequence;
	}

	public struct DBCall
	{
		[PRIMARY_KEY, AUTOINCREMENT, NOT_NULL, UNIQUE]
		public int			id;
		public string		parentType;
		public int			parentId;
		public string		callName;
		public int			sequence;
	}

	public struct DBPhysicalData
	{
		[PRIMARY_KEY, AUTOINCREMENT, NOT_NULL, UNIQUE]
		public int			id;
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

    public class DBOverride
    {
        [PRIMARY_KEY, AUTOINCREMENT, NOT_NULL, UNIQUE]
        public int id;
    }

    public class TypeA : DBOverride
    {
        public string type;
    }
}
