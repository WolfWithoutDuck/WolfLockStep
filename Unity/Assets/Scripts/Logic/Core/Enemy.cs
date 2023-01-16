using Lockstep.Math;
using UnityEngine.AI;

namespace Lockstep.Logic
{
    public class Enemy : BaseEntity
    {
        public CBrain CBrain;
        public NavMeshAgentMono nav;
        public IEnemyView eventHandler;
        public Player player;
        public int scoreValue = 10;
        public int attackDamage = 10;
        public bool playerInRange;
        public LFloat timer;
        public bool isSinking;
        public LFloat timeBetweenAttacks = LFloat.half;

        public override void DoUpdate(LFloat deltaTime)
        {
            base.DoUpdate(deltaTime);
            var minDist = LFloat.MaxValue;
            // foreach (var VARIABLE in GameManger)
            // {
            //     
            // }
        }
    }
}