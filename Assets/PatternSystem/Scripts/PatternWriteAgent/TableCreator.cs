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

    public static void PushTable(System.Type [] types)
    {
        for (int i = 0; i < types.Length; ++i)
        {
            s_objects.Add(types[i]);
        }
    }

    protected TableCreator()
    {
		if(!s_objects.Contains(this.GetType()))
        	s_objects.Add(this.GetType());
    }
    public FieldInfo[] GetMembers()
	{
		System.Type type = GetType();
		FieldInfo[] members = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
		return members;
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
    



