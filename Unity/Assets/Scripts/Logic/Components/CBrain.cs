using AIToolkitDemo;
using Lockstep.BehaviourTree;
using Lockstep.Math;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Lockstep.Logic
{
    public class CBrain
    {
        public const string BBKEY_NEXTMOVINGPOSITION = "NextMovingPosition";
        public BaseEntity owner;
        private BTAction _behaviorTree;
        public BTInfo BtInfo { get; protected set; }
        public AIEntityWorkingData BehaviourWorkingData { get; protected set; }

        private GameObject _targetDummyObject;
        
        private string _lastTriggeredAnimation;
        private BlackBoard _blackboard;

        public bool isDead => owner.isDead;

        public void Inti(BaseEntity owner, int id)
        {
            this.owner = owner;
            _behaviorTree = BtInfo.RootNode;
            BehaviourWorkingData = new AIEntityWorkingData();
            BehaviourWorkingData.entity = this;
            BehaviourWorkingData.Init(BtInfo.Offsets,BtInfo.MemSize);
            BehaviourWorkingData.entityTF = owner.transform;
            BehaviourWorkingData.EntityAnimatorView = new CAnimatorView().Init(owner);
            _blackboard = new BlackBoard();
            _lastTriggeredAnimation = string.Empty;
        }

        public T GetBBValue<T>(string key, T defaultValue)
        {
            return _blackboard.GetValue(key, defaultValue);
        }

        public void PlayAnimation(string name)
        {
            if (_lastTriggeredAnimation == name)
            {
                return;
            }

            _lastTriggeredAnimation = name;
            BehaviourWorkingData.EntityAnimatorView.SetTrigger(name);
        }

        public void DoUpdate(LFloat deltaTime)
        {
            BehaviourWorkingData.EntityAnimatorView.speed = (LFloat)1;
            BehaviourWorkingData.deltaTime = deltaTime;
            BehaviourWorkingData.ClearRunTimeInfo();
            //TODO 黑板数据设置玩家当前位置
            _blackboard.SetValue(BBKEY_NEXTMOVINGPOSITION,Vector3.zero);
            if (_behaviorTree.Evaluate(BehaviourWorkingData))
            {
                _behaviorTree.Update(BehaviourWorkingData);
            }
            else
            {
                _behaviorTree.Transition(BehaviourWorkingData);
            }
        }
        
    }
}