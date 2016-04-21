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

	public class DBHabit
	{
		[PRIMARY_KEY, AUTOINCREMENT, NOT_NULL, UNIQUE]
		public int 		id;
	}

	public class DBTriger
	{
		[PRIMARY_KEY, AUTOINCREMENT, NOT_NULL, UNIQUE]
		public int 		id;
		public int 		habitId;
		public string 		tirigerName;
	}

	public class DBArrange
	{
		[PRIMARY_KEY, AUTOINCREMENT, NOT_NULL, UNIQUE]
        public int id;
		public int			parentType;
		public int			parentId;
		public int			type;
		public int			repeat;
		public float		sequence;
	}

	public class DBTimer
	{
		[PRIMARY_KEY, AUTOINCREMENT, NOT_NULL, UNIQUE]
		public int			id;
		public string		parentType;
		public int			parentId;
		public int			isRelative;
		public float		time;
		public int			sequence;
	}

	public class DBCall
	{
		[PRIMARY_KEY, AUTOINCREMENT, NOT_NULL, UNIQUE]
		public int			id;
		public string		parentType;
		public int			parentId;
		public string		callName;
		public int			sequence;
	}

	public class DBPhysicalData
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
}
