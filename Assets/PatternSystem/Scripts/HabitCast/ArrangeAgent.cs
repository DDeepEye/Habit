﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PatternSystem
{
    public class ArrangeAgent : AttributeAgent 
    {
        public const string DBType = "Arrange";


        public ArrangeAgent()
        {
            _attributeType = ePatternList.ARRANGE;
        }


		public Arrange.ArrangeType _type = Arrange.ArrangeType.SERIES;
		public int _repeat = 1;


        public override Container GetContainer(GameObject target)
		{
            List<Container> containers = new List<Container> ();
			List<AttributeAgent> attributes = AttributeAgent.CollectAttribute(gameObject.transform);
			foreach (AttributeAgent att in attributes)
			{
                containers.Add(att.GetContainer (target));
			}

            Arrange p = new Arrange (target, _type, containers, _repeat);

			return p;
		}
	}
}


