using Lockstep.Math;

namespace Lockstep.Logic
{
    public partial class Player : BaseEntity
    {
        public int localId;
        public IPlayerView eventHandler;
        public bool damaged;
        public CMover CMover = new CMover();
        public SkillShoot SkillShoot = new SkillShoot();
        public PlayerInput InputAgent = new PlayerInput();


        public Player()
        {
            RegisterComponent(CMover);
            RegisterComponent(SkillShoot);
        }

        private LFloat normalSpd = new LFloat(5);
        private LFloat fastSpd = new LFloat(8);
        private LFloat spdUpTimer;

        public override void DoUpdate(LFloat deltaTime)
        {
            if (InputAgent.isSpeedUp)
            {
                spdUpTimer = new LFloat(3);
            }

            spdUpTimer -= deltaTime;
            speed = spdUpTimer > 0 ? fastSpd : normalSpd;
            if (spdUpTimer > 0)
            {
                //ToDo PostEffectManager放特效 
            }

            var skillId = InputAgent.skillId;
            if (skillId >= 0 && !isFire)
            {
                if (skillId < this.allSkills.Count)
                {
                    allSkills[skillId].Fire();
                }
            }

            base.DoUpdate(deltaTime);

            if (!isFire)
            {
                eventHandler.Animating(CMover.hasReachTarget);
            }

            animator.DoLaterUpdate(deltaTime);
        }

        protected override void OnTakeDamage(int amount, LVector3 hitPoint)
        {
            damaged = true;
            currentHealth -= amount;
            eventHandler.TakeDamage(amount, transform.Pos3);
            OnBeAtked?.Invoke(amount, transform.Pos3);
            //ToDo 抛事件玩家被攻击
            if (currentHealth <= 0 && !isDead)
            {
                isDead = true;
                eventHandler.OnDead();
                CollisionManager.Instance.RemoveCollider(this);
            }
        }
    }
}