﻿using System;
using Lockstep.BehaviourTree;

namespace AIToolkitDemo
{
    [AttributeUsage(AttributeTargets.Class,Inherited = false)]
    public class AINodeAttribute : StringInfoAttribute
    {
        public AINodeAttribute(string menu, EBTTypeIdxAI idx) : base(menu, (int)idx)
        {
        }
    }
}