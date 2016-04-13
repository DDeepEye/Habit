using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DBAgent;
using PatternSystem;

public abstract class ClassMemberGetter
{
    protected static HashSet<System.Type> s_objects = new HashSet<System.Type>();

    public static HashSet<System.Type> Objects
	{ 
		get 
		{
			if(!s_objects.Contains(typeof(PatternSystem.DBHabit)))
				s_objects.Add(typeof(PatternSystem.DBHabit));
			if(!s_objects.Contains(typeof(PatternSystem.DBTriger)))
				s_objects.Add(typeof(PatternSystem.DBTriger));
			if(!s_objects.Contains(typeof(PatternSystem.DBArrange)))
				s_objects.Add(typeof(PatternSystem.DBArrange));
			if(!s_objects.Contains(typeof(PatternSystem.DBPhysicalData)))
				s_objects.Add(typeof(PatternSystem.DBPhysicalData));
			if(!s_objects.Contains(typeof(PatternSystem.DBTimer)))
				s_objects.Add(typeof(PatternSystem.DBTimer));
            if (!s_objects.Contains(typeof(PatternSystem.TypeA)))
                s_objects.Add(typeof(PatternSystem.TypeA));
			return s_objects; 
		} 
	}

    protected ClassMemberGetter()
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
    
public class Timer : ClassMemberGetter
{
    private float time = 0.0f;
}

public class Message : ClassMemberGetter
{
    private string msg = "64";
}

public struct structSample 
{
	public enum ELIST
	{
		E1,
		E2,
	}

	public ELIST e;
	public string msg;
	public float time;
}

public class TableCreator : MonoBehaviour {

	// Use this for initialization
	void Start () {
		/*
        Timer t = new Timer();
        Message m = new Message();
        FieldInfo[] tlist = t.GetMembers();
        FieldInfo[] mlist = m.GetMembers();

        for (int i = 0; i < tlist.Length; ++i )
        {
            Debug.Log("time member type  = " + tlist[i].FieldType.Name + "    time member name = " + tlist[i].Name);
        }

        for (int i = 0; i < mlist.Length; ++i)
        {
            Debug.Log("msg member type  = " + mlist[i].FieldType.Name + "    msg member name = " + mlist[i].Name);
            Debug.Log("msg member value  = " + mlist[i].GetValue(m));
        }

        foreach (System.Type c in ClassMemberGetter.Objects)
        {
            Debug.Log("class name  = " + c.Name);
        }

		FieldInfo[] fields = typeof(structSample).GetFields ();
		for (int i = 0; i < fields.Length; ++i )
		{
			
			Debug.Log("fields member type  = " + fields[i].FieldType.Name + "    fields member name = " + fields[i].Name);
		}
		*/



        ///Debug.Log("msg member type " + mlist[0].GetType().ToString() + "msg member name" + mlist[0].ToString());

		/*
		FieldInfo[] fields = typeof(DBTimer).GetFields ();
		for (int i = 0; i < fields.Length; ++i) 
		{
			Debug.Log ("FieldName = "+fields[i].Name);
			int attlength = fields [i].GetCustomAttributes (true).Length;
			Debug.Log ("Attribute Num = " + attlength.ToString ());

			string attributes = DBFieldAttribute.GetDBAttributes (fields[i]);
			Debug.Log ("Attributes = " + attributes);

		}
		*/
 
        /*
		MonoSQLiteManager msm = new MonoSQLiteManager("/PatternSystem/Resources/DB/PatternSystem.db");        
		ClassMemberGetter.CreateTable (msm);
        msm.DQDeleteColumn("DBTimere","tt");
         * */


	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
