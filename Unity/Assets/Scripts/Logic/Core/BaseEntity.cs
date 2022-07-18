using System;
using Lockstep.Collision2D;
using Lockstep.Math;

namespace Lockstep.Logic
{
    public interface IUpdate
    {
        void DoUpdate(LFloat delataTime);
    }

    public interface IAwake
    {
        void DoAwake();
    }

    public interface IStart
    {
        void DoStart();
    }

    public interface IDestroy
    {
        void DoDestroy();
    }

    public interface ILifeCycle : IUpdate, IAwake, IStart, IDestroy
    {
    }
    
    [Serializable]
    public class BaseLifeCycle : ILifeCycle
    {
        public virtual void DoUpdate(LFloat delataTime)
        {
        }

        public virtual void DoAwake()
        {
        }

        public virtual void DoStart()
        {
        }

        public virtual void DoDestroy()
        {
        }
    }


    public interface IEntity : ILifeCycle
    {
        
    }

    public class BaseEntity : BaseLifeCycle,IEntity,ILPTriggerEventHandler
    {
        public void OnLPTriggerEnter(ColliderProxy other)
        {
            throw new NotImplementedException();
        }

        public void OnLPTriggerStay(ColliderProxy other)
        {
            throw new NotImplementedException();
        }

        public void OnLPTriggerExit(ColliderProxy other)
        {
            throw new NotImplementedException();
        }
    }
}