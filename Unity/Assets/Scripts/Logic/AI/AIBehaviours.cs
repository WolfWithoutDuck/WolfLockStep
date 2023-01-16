using System.Collections.Generic;
using Lockstep.BehaviourTree;
using Lockstep.Collision2D;
using Lockstep.Logic;
using Lockstep.Math;
using UnityEngine;

namespace AIToolkitDemo
{
    public class AIEntityWorkingData : BTWorkingData
    {
        public CBrain entity { get; set; }
        public CTransform2D entityTF { get; set; }
        public CAnimatorView EntityAnimatorView { get; set; }
        public LFloat gameTime { get; set; }
        public LFloat deltaTime { get; set; }
    }

    public class AIEntityBehaviourTreeFactory
    {
        private static Dictionary<int, BTInfo> _id2BtInfo = new Dictionary<int, BTInfo>();
        private static string relPath = "Resources/AI/";
        private static string relResPath = "AI/";

        public static BTInfo GetBtInfo(int id)
        {
            if (_id2BtInfo.TryGetValue(id, out var val))
            {
                return val;
            }

            var info = CreateBtInfo(id);
            _id2BtInfo[id] = info;
            return info;
        }

        static BTInfo CreateBtInfo(int id)
        {
            var textAsset = Resources.Load<TextAsset>(relResPath + id);
            var bytes = textAsset.bytes;
            var info = BTInfo.Deserialize(bytes, BTFactoryAI.CreateNode);
            return info;
        }

        static BTInfo LoadBehaviorTreeDemo()
        {
            var info = new BTInfo();
            info.Id = 0;
            info.Description = "asdfasf";
            BTFactory.BeforeCreateNode();
            var bt = Create<BTActionPrioritizedSelector>();
            bt
                .AddChild(Create<BTActionSequence>()
                    .SetPrecondition((BTPreconditionNot)Create<BTPreconditionNot>()
                        .AddChild(Create<CON_HasReachedTarget>()))
                    .AddChild(Create<NOD_TurnTo>())
                    .AddChild(Create<NOD_MoveTo>()))
                .AddChild(Create<BTActionSequence>()
                    .AddChild(Create<NOD_TurnTo>())
                    .AddChild(Create<NOD_Attack>()));
            info.RootNode = bt;
            info.Init();
            return info;
        }

        public static T Create<T>() where T : BTNode, new()
        {
            return BTFactory.CreateNode<T>();
        }
    }
    
    
    [AINode("HasReachedTarget", EBTTypeIdxAI.CON_HasReachedTarget)]
    public partial class CON_HasReachedTarget : BTConditionAI
    {
        public override bool IsTrue(BTWorkingData wData)
        {
            return true;
        }
    }
    
    [AINode("Attack", EBTTypeIdxAI.NOD_Attack)]
    public unsafe partial class NOD_Attack : BTActionLeafAI
    {
        
    }
    
    [AINode("NOD_MoveTo", EBTTypeIdxAI.NOD_MoveTo)]
    public partial class NOD_MoveTo : BTActionLeafAI
    {
        protected override void OnEnter(BTWorkingData wData)
        {
            var thisData = wData.As<AIEntityWorkingData>();
            // if(thisData.entity.isDead)
        }

        protected override int OnExecute(BTWorkingData wData)
        {
            return base.OnExecute(wData);
        }
    }

    [AINode("NOD_TurnTo", EBTTypeIdxAI.NOD_TurnTo)]
    public partial class NOD_TurnTo : BTActionLeafAI
    {
        protected override void OnEnter(BTWorkingData wData)
        {
            base.OnEnter(wData);
        }

        protected override int OnExecute(BTWorkingData wData)
        {
            return base.OnExecute(wData);
        }
    }
}