﻿using UnityEngine;
using System.Collections;


namespace PatternSystem
{
    public class Scale : Physical
    {
        public Scale()
        {
            _attributeType = EditorPrefabList.SCALE;
        }

        public override void Run(GameObject target)
        {
        }
    }
}
