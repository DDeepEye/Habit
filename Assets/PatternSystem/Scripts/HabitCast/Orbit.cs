﻿using UnityEngine;
using System.Collections;

namespace PatternSystem
{
    public class Orbit : Physical
    {
        public Orbit()
        {
            _attributeType = EditorPrefabList.ORBIT;
        }
    }
}