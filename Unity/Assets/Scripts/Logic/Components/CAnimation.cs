using System;
using System.Collections.Generic;
using Lockstep.Logic;
using Lockstep.Math;
using UnityEngine;
using Debug = Lockstep.Logging.Debug;

public class CAnimation : MonoBehaviour, IView
{
    [Header("shaderDebug")] public float shaderDuration;
    public float shaderFadeInTime;
    public float shaderFadeOutTime;
    public float shaderSampleDist;
    public float shaderSampleStrength;

    [Header("Comps")] public Animation animComp;
    public Transform rootTrans;
    public AnimatorConfig config;

    [Header("Other")] public List<string> animNames = new List<string>();
    public string CurAnimName;
    public int curAnimIdx = -1;

    [HideInInspector] public List<AnimInfo> animInfos => config.anims;
    [HideInInspector] public AnimationState animState;
    [HideInInspector] public LFloat animLen;
    [HideInInspector] public LFloat timer;
    [HideInInspector] public AnimInfo CurAnimInfo;
    [HideInInspector] public AnimBindInfo CurAnimBindInfo;

    private LVector3 initPos;
    public LFloat debugTimer = LFloat.zero;

    private void Start()
    {
        if (animComp == null)
        {
            animComp = GetComponent<Animation>();
            if (animComp == null)
            {
                animComp = GetComponentInChildren<Animation>();
            }
        }

        animNames.Clear();
        foreach (var info in animInfos)
        {
            animNames.Add(info.name);
        }

        initPos = transform.position.ToLVector3();
        play(AnimDefine.Idle);
    }

    public void SetTrigger(string name, bool isCrossfade = false)
    {
        play(name, isCrossfade);
    }

    private void play(string name, bool isCrossfade = false)
    {
        if (CurAnimName == name)
        {
            return;
        }

        var idx = animNames.IndexOf(name);
        if (idx == -1)
        {
            UnityEngine.Debug.Log("miss animation " + name);
            return;
        }

        Debug.Trace($"{owner.EntityId} PlayAnim {name} rawName {CurAnimName}");

        var hashChangedAnim = CurAnimName != name;
        CurAnimName = name;
        animState = animComp[CurAnimName];
        CurAnimInfo = animInfos[idx];
        CurAnimBindInfo = config.events.Find(a => a.name == name);
        if (CurAnimBindInfo == null)
        {
            CurAnimBindInfo = AnimBindInfo.Empty;
        }

        if (hashChangedAnim)
        {
            ResetAnim();
        }

        var state = animComp[CurAnimName];
        if (state != null)
        {
            if (isCrossfade)
            {
                animComp.CrossFade(CurAnimName);
            }
            else
            {
                animComp.Play(CurAnimName);
            }
        }
    }

    private BaseEntity owner;

    public void BindEntity(BaseEntity entity)
    {
        owner = entity;
    }

    public void SetTime(LFloat timer)
    {
        int idx = GetTimeIdx(timer);
        initPos = owner.transform.Pos3 - CurAnimInfo[idx].pos;
        Debug.Trace(
            $"{owner.EntityId} SetTime  idx:{idx} intiPos {owner.transform.Pos3}",
            true);
        this.timer = timer;
    }

    public void LateUpdate()
    {
        if (CurAnimBindInfo != null && CurAnimBindInfo.isMoveByAnim)
        {
            rootTrans.localPosition = Vector3.zero;
        }
    }

    public void DoLaterUpdate(LFloat deltaTime)
    {
        animLen = CurAnimInfo.length;
        timer += deltaTime;
        if (timer > animLen)
        {
            ResetAnim();
        }

        if (!Application.isPlaying)
        {
            sample(timer);
        }

        UpdateTrans();
    }

    private void UpdateTrans()
    {
    }

    private void sample(LFloat time)
    {
        if (animState == null)
        {
            return;
        }

        if (!Application.isPlaying)
        {
            animComp.Play();
        }

        animState.enabled = true;
        animState.weight = 1;
        animState.time = time.ToFloat();
        animComp.Sample();
        if (!Application.isPlaying)
        {
            animState.enabled = false;
        }
    }

    private int GetTimeIdx(LFloat lFloat)
    {
        throw new NotImplementedException();
    }


    private void ResetAnim()
    {
        timer = LFloat.zero;
        SetTime(LFloat.zero);
    }
}