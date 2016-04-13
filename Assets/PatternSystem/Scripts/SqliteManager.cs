using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DBAgent;

public class SqliteManager : MonoBehaviour {	

    public void Start()
    {
        MonoSQLiteManager msm = new MonoSQLiteManager("/PatternSystem/Resources/DB/PatternSystem.db");

        PatternSystem.DBArrange aba = new PatternSystem.DBArrange();
        aba.parentId = 0;
        aba.parentType = 2;
        aba.repeat = 3;
        aba.sequence = 5;
        aba.type = 1;
        msm.InsertTable<PatternSystem.DBArrange>(ref aba);
        msm.CommandQueries();
        /*
        msm.CreateTable(typeof(PatternSystem.DBArrange));
        msm.CommandQueries();
        msm.Close();
         */
    }

    public void Update()
    {

    }
}
