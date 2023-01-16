using System;
using System.Collections.Generic;
using Lockstep.Math;
using UnityEngine;

namespace Lockstep.Logic
{
    public class CSkill : MonoBehaviour
    {
        // public BaseEntity owner => view?.ow;

        public LFloat CD;
        public LFloat cdTimer;
        public EAnimDefine animName;
        public KeyCode fireKeyCode;
        public LFloat doneDelay;
        public int targetLayer;
        public List<SkillPart> parts = new List<SkillPart>();

        public enum ESkillState
        {
            Idle,
            Firing,
        }

        private ESkillState state;
        [Header("__Debug")] [SerializeField] public LFloat maxPartTime;
        [SerializeField] private LFloat skillTimer;
        [SerializeField] private bool __DebugFireOnce = false;
        private PlayerView view;


        public void DoUpdate(LFloat deltaTime)
        {
            
        }

        public void Fire()
        {
            throw new NotImplementedException();
        }
    }
}