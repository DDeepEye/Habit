using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using PatternSystem;
using DBAgent;

public class InitData : MonoBehaviour {	

    public void Start()
    {
		/*
        Type type = typeof(PatternSystem.DBArrange);
        PatternSystem.DBArrange arrange = Activator.CreateInstance(type) as PatternSystem.DBArrange;
        arrange.sequence = 6.0f;
         * */

        /*
        MonoSQLiteManager msm = new MonoSQLiteManager("/PatternSystem/Resources/DB/PatternSystem.db");
		

        System.Type[] table = {
                                typeof( PatternSystem.DBArrange ),
                                typeof( PatternSystem.DBHabit ),
                                typeof( PatternSystem.DBTriger ),
                                typeof( PatternSystem.DBTimer ),
                                typeof( PatternSystem.DBCall ),
                                typeof( PatternSystem.DBPhysicalData ),
                                };

        TableCreator.PushTables(table);
        TableCreator.CreateTable(msm);
        */




        /*
        msm.CreateTable(typeof(PatternSystem.DBArrange));
        msm.CommandQueries();
        msm.Close();
         */

        ///MonoSQLiteManager msm = new MonoSQLiteManager("/PatternSystem/Resources/DB/PatternSystem.db");
        /*
        PatternSystem.DBArrange ar = msm.GetTableLastData<PatternSystem.DBArrange>();
        Debug.Log("DBArrange Last row id = " + ar.id.ToString());
        */


        Dictionary<int, DBBaseTable> arranges = ResourcesPool.Instance.GetTableData<DBArrange>();
        Debug.Log("DBArrange Count id = " + arranges.Count);




        /*


        arranges[1].sequence = 100.334f;
        PatternSystem.DBArrange aar = arranges[1];

        msm.UpdateTable<PatternSystem.DBArrange>(ref aar);
        msm.CommandQueries();

        arranges = msm.GetTableData<PatternSystem.DBArrange>();
        foreach (PatternSystem.DBArrange ar in arranges)
        {
            Debug.Log("row id = " + ar.id+", sequence = "+ar.sequence );
        }
        */

    }




    public void Update()
    {

    }
}
