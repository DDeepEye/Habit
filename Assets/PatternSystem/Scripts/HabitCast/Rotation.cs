﻿using UnityEngine;
using System.Collections;

namespace PatternSystem
{
    public class Rotation : Physical
    {
        public Rotation()
        {
            _attributeType = EditorPrefabList.ROTATION;
        }

        public override void Run(GameObject target)
        {
        }
    }
}

