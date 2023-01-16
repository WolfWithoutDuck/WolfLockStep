using System;
using System.Collections;
using System.Collections.Generic;
using Lockstep;
using Lockstep.Collision2D;
using Lockstep.Logic;
using Lockstep.Math;
using UnityEngine;

public class NavMeshAgentMono : MonoBehaviour
{
    public BaseEntity owner;
    public CNavMesh agent = new CNavMesh();
    private CTransform2D Transform => owner.transform;

    public void DoStart()
    {
        agent.FuncOnReachTargetPos += OnReachTargetPos;
        agent.transform2D = Transform;
        Transform.Pos3 = transform.position.ToLVector3();
    }

    public void Update()
    {
        Transform.Pos3 = transform.position.ToLVector3();
        agent.DoUpdate(Time.deltaTime.ToLFloat());
        transform.position = Transform.Pos3.ToVector3();
        transform.LookAt(transform.position + agent.curNormal.ToVector3());
    }

    public void SetDestination(Vector3 targetPos)
    {
        SetDestination(targetPos.ToLVector3());
    }

    public void SetDestination(LVector3 targetPos)
    {
        if (isDebug)
        {
            int i = 0;
        }

        Transform.Pos3 = transform.position.ToLVector3();
        agent.SetDistination(targetPos);
    }

    void OnReachTargetPos()
    {
    }

    public bool isDebug = false;

    private void OnDrawGizmos()
    {
        if (!isDebug)
            return;
        agent.OnDrawGizmos();
    }
}