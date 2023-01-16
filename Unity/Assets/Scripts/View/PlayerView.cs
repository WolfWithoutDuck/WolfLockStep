using Lockstep.Math;
using UnityEngine;

namespace Lockstep.Logic
{
    public partial class PlayerView :MonoBehaviour,IPlayerView
    {
        public SkillBoxConfig skillBoxConfig;
        // public Player 
        
        
        public void BindEntity(BaseEntity entity)
        {
            throw new System.NotImplementedException();
        }

        public void TakeDamage(int amount, LVector3 hitPoint)
        {
            throw new System.NotImplementedException();
        }

        public void OnDead()
        {
            throw new System.NotImplementedException();
        }

        public void SetLinePosition(int index, LVector3 position)
        {
            throw new System.NotImplementedException();
        }

        public void Shoot()
        {
            throw new System.NotImplementedException();
        }

        public void DisableEffects()
        {
            throw new System.NotImplementedException();
        }

        public void Animating(bool isIdle)
        {
            throw new System.NotImplementedException();
        }
    }
}