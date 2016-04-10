using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DBAgent;

public class SqliteManager : MonoBehaviour {	

    public void Start()
    {
        MonoSQLiteManager msm = new MonoSQLiteManager("/PatternSystem/Resources/DB/PatternSystem.db");
        Dictionary<string, ColumnInfo> t_arrange = msm.GetDBTableColumnsInfo("DBArrange");
        msm.Close();
    }

    public void Update()
    {

    }
}
