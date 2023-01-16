using System;
using Lockstep.Collision2D;
using Lockstep.Math;
using UnityEngine;

[Serializable]
public class CNavMesh
{
    [Header("Steering")] public LFloat Speed;
    public LFloat AngularSpeed;
    public LFloat Acceleration;
    public LFloat StoppingDisctance;
    public bool AutoBraking;

    [Header("Obstacle Avoidance")] public LFloat Radius;
    public LFloat Height;
    public LFloat Priority;

    [Header("PathFinding")] public bool AutoTraverseOffMeshLink;
    public bool AutoRepath;

    public CTransform2D transform2D;

    private LFloat _minSqrReseachDist => (StoppingDisctance * StoppingDisctance) / 4;
    private LVector3 _targetPos;

    // public LVector3 TargetPos
    // {
    //     set =>
    // }

    private LFloat _curSegLen;
    private LFloat _distInCurSeg;
    private int _pointCount;
    private int _nextPointIdx;
    private LVector3[] _path;

    private LVector3 _nextPoint;
    private LVector3 _prePoint;
    public LVector3 curNormal;

    public Action FuncOnReachTargetPos;
    public bool enable = true;
    private bool isFirst = true;

    public void SetDistination(LVector3 targetPos)
    {
        var preTargetPos = _targetPos;
        _targetPos = targetPos;
        if (isFirst || (_targetPos - preTargetPos).sqrMagnitude > _minSqrReseachDist)
        {
            //TODO 寻路
        }

        if (isFirst)
        {
            isFirst = false;
        }
    }

    public void DoUpdate(LFloat deltaTime)
    {
        if (!enable)
        {
            return;
        }

        if (_path != null)
        {
            var dist = Speed * deltaTime;
            _distInCurSeg = dist + _distInCurSeg;
            while (_distInCurSeg > _curSegLen)
            {
                if (_nextPointIdx >= _pointCount - 1)
                {
                    OnFinishPath();
                    return;
                }

                _distInCurSeg -= _curSegLen;
                NextPoint();
            }

            transform2D.Pos3 = _prePoint + curNormal * _distInCurSeg;
            if ((transform2D.Pos3 - _targetPos).magnitude < StoppingDisctance)
            {
                OnFinishPath();
            }
        }
    }

    private void NextPoint()
    {
        try
        {
            _nextPointIdx++;
            _nextPoint = _path[_nextPointIdx];
            _prePoint = _path[_nextPointIdx - 1];
            curNormal = (_nextPoint - _prePoint).normalized;
            _curSegLen = (_nextPoint - _prePoint).magnitude;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private void OnFinishPath()
    {
        _path = null;
        _pointCount = 0;
        FuncOnReachTargetPos?.Invoke();
    }

    void FindPath(bool froceSet = false)
    {
        //TODO 寻路组件
        // var tPath = 
    }

    public void OnDrawGizmos()
    {
        
    }
}