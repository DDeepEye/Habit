using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DBAgent;
using PatternSystem;

public abstract class TableCreator
{
    protected static HashSet<System.Type> s_objects = new HashSet<System.Type>();

    public static HashSet<System.Type> Objects
	{ 
		get 
		{
			return s_objects; 
		} 
	}

    public static void PushTables(System.Type [] tables)
    {
        for (int i = 0; i < tables.Length; ++i)
        {
            s_objects.Add(tables[i]);
        }
    }

	static public void CreateTable(MonoSQLiteManager sqlManager)
	{
		foreach (System.Type table in Objects)
		{
			sqlManager.CreateTable (table);
		}
        sqlManager.CommandQueries();
	}

}
    



