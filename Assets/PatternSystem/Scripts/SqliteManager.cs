using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DBAgent;

public class SqliteManager : MonoBehaviour {	

    public void Start()
    {
        MonoSQLiteManager msm = new MonoSQLiteManager("/PatternSystem/Resources/DB/PatternSystem.db");
		msm.DeleteColumn("backup_sample", "nameTEXT");
        msm.Close();
    }

    public void Update()
    {

    }
}
