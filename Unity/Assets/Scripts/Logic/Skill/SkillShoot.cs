using Lockstep.Collision2D;
using Lockstep.Math;

namespace Lockstep.Logic
{
    public partial class SkillShoot : PlayerComponent
    {
        public CTransform2D shootTrans = new CTransform2D { y = LFloat.half };
        public IPlayerView EventHandler => ((Player)entity).eventHandler;
        public LFloat effectDisplayTime = new LFloat(true, 100);
        public int PrefabId;
        public int damagePerShot = 20;
        public LFloat timeBetweenBullets;
        public LFloat range;
        public int shootableMask;
        public LFloat timer;
        
        public bool isInputFire => input.isInputFire;

        public override void DoUpdate(LFloat delataTime)
        {
            timer += delataTime;
            if (isInputFire)
            {
                if (timer >= timeBetweenBullets)
                {
                    timer = LFloat.zero;
                    Shoot();
                }
            }

            if (timer >= timeBetweenBullets * effectDisplayTime)
            {
                EventHandler.DisableEffects();
            }
        }

        private void Shoot()
        {
            timer = LFloat.zero;
            EventHandler.Shoot();
            var shootRay = new Ray2D() { origin = transform.pos, direction = transform.forward };
            
        }
    }
}