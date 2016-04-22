using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DBAgent;

public class SqliteManager : MonoBehaviour {	

    public void Start()
    {
		/*
        Type type = typeof(PatternSystem.DBArrange);
        PatternSystem.DBArrange arrange = Activator.CreateInstance(type) as PatternSystem.DBArrange;
        arrange.sequence = 6.0f;
         * */
        /*
        MonoSQLiteManager msm = new MonoSQLiteManager("/PatternSystem/Resources/DB/PatternSystem.db");
		List<PatternSystem.DBArrange> arranges = msm.GetTableData<PatternSystem.DBArrange> ();
		if (arranges != null)
        {
            
		}

        System.Type[] types = {
                                typeof( PatternSystem.DBArrange ),
                                typeof( PatternSystem.DBHabit ),
                                typeof( PatternSystem.DBTriger ),
                                typeof( PatternSystem.DBTimer ),
                                typeof( PatternSystem.DBCall ),
                                typeof( PatternSystem.DBPhysicalData ),
                                };

        TableCreator.PushTable(types);
        TableCreator.CreateTable(msm);
        */


        /*
        msm.CreateTable(typeof(PatternSystem.DBArrange));
        msm.CommandQueries();
        msm.Close();
         */

        MonoSQLiteManager msm = new MonoSQLiteManager("/PatternSystem/Resources/DB/PatternSystem.db");
        /*
        PatternSystem.DBArrange ar = msm.GetTableLastData<PatternSystem.DBArrange>();
        Debug.Log("DBArrange Last row id = " + ar.id.ToString());
        */

        List<PatternSystem.DBArrange> arranges = msm.GetTableData<PatternSystem.DBArrange>();
        foreach (PatternSystem.DBArrange ar in arranges)
        {
            Debug.Log("row id = " + ar.id+", sequence = "+ar.sequence );
        }

        arranges[1].sequence = 100.334f;
        PatternSystem.DBArrange aar = arranges[1];

        msm.UpdateTable<PatternSystem.DBArrange>(ref aar);
        msm.CommandQueries();

        arranges = msm.GetTableData<PatternSystem.DBArrange>();
        foreach (PatternSystem.DBArrange ar in arranges)
        {
            Debug.Log("row id = " + ar.id+", sequence = "+ar.sequence );
        }

    }




    public void Update()
    {

    }
}
