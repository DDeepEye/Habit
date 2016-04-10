
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class DBTableList{
	protected static HashSet<System.Type> s_tables = new HashSet<System.Type>();

	public static HashSet<System.Type> Tables { get { return s_tables; } }

	public static void PushTable(System.Type type)
	{
		if(s_tables.Contains(type))
			s_tables.Add(type);
	}
}
