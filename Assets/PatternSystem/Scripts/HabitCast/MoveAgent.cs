﻿using UnityEngine;
using System.Collections;


namespace PatternSystem
{
    public class MoveAgent : PhysicalAgent
    {
        public const string DBType = "Move";
        public MoveAgent()
        {
            _attributeType = ePatternList.MOVE;
        }
        public override Container GetContainer(GameObject target)
		{
			return new Move(target, _value, _time, _type);
		}
	}

}
