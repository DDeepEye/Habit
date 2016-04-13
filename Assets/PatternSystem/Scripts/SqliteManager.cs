using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DBAgent;

public class SqliteManager : MonoBehaviour {	

    public void Start()
    {
        MonoSQLiteManager msm = new MonoSQLiteManager("/PatternSystem/Resources/DB/PatternSystem.db");
        msm.CreateTable(typeof(PatternSystem.DBArrange));
        msm.CommandQueries();
        msm.Close();
    }

    public void Update()
    {

    }
}
