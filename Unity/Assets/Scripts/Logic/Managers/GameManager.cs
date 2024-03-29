﻿using System;
using System.Collections.Generic;
using Lockstep.FakeServer;
using Lockstep.Game;
using Lockstep.Logic;
using Lockstep.Math;
using Lockstep.PathFinding;
using Debug = Lockstep.Logging.Debug;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

namespace Logic.Managers
{
    public class GameManager : BaseManager
    {
        public static GameManager Instance { get; private set; }
        [Header("Collision")] public CollisionManager collisionManager;
        public int maxBspDepth;
        public int maxBspDepthNodeId;

        [Header("Configs")] public PlayerServerInfo playerConfig;
        public int maxEnemyCount = int.MaxValue;
        public Material debugMat;

        [Header("Recoder")] public bool IsReplay = false;
        public string recordFilePath;

        [Header("frame datas")] public int mapId;
        private bool hasStart = false;
        private int inputTick;
        public int predictTickCount = 3;
        public static int maxServerFrameIdx;
        public List<FrameInput> frames = new List<FrameInput>();
        public int curFrameIdx = 0;
        public int localPlayerId = 0;
        public int playerCount = 1;
        public int curMapId = 0;
        public PlayerServerInfo[] PlayerServerInfos;
        public FrameInput CurFrameInput;

        [Header("ping")] public static int pingVal;
        public static List<float> delays = new List<float>();
        public Dictionary<int, float> tick2SendTimer = new Dictionary<int, float>();

        [Header("game data")] public static List<Player> allPlayers = new List<Player>();
        public static Player player;
        public static Transform playerTrans;
        public float remainTime;
        private List<BaseManager> _mgrs = new List<BaseManager>();

        public NetClient netClient;

        private static string _traceLogPath
        {
            get
            {
#if UNITY_STANDALONE_OSX
            return $"/tmp/LPDemo/Dump_{Instance.localPlayerId}.txt";
#else
                return $"c:/tmp/LPDemo/Dump_{Instance.localPlayerId}.txt";
#endif
            }
        }

        public void RegisterManagers(BaseManager mgr)
        {
            _mgrs.Add(mgr);
        }

        private void Awake()
        {
            Screen.SetResolution(1024, 768, false);
            gameObject.AddComponent<PingMono>();
            _Awake();
        }

        private void _Awake()
        {
#if !UNITY_EDITOR
            IsReplay = false;
#endif
            DoAwake();
            foreach (var mgr in _mgrs)
            {
                mgr.DoAwake();
            }
        }

        private void Start()
        {
            _Start();
        }

        private void _Start()
        {
            DoStart();
            foreach (var mgr in _mgrs)
            {
                mgr.DoStart();
            }

            Debug.Trace("Before StartGame _IdCounter" + BaseEntity._IdCounter);
            if (!IsReplay)
            {
                netClient = new NetClient();
                netClient.Start();
                netClient.Send(new Msg_JoinRoom() { name = Application.dataPath });
            }
            else
            {
                StartGame(0, PlayerServerInfos, localPlayerId);
            }
        }

        private void Update()
        {
            _DoUpdate();
        }

        private void _DoUpdate()
        {
            if (!hasStart)
                return;
            remainTime += Time.deltaTime;
            while (remainTime >= 0.03f)
            {
                if (!IsReplay)
                {
                    SendInput();
                }

                if (GetFrame(curFrameIdx) == null)
                {
                    return;
                }

                Step();
            }
        }

        private void SendInput()
        {
            predictTickCount = 2;
            if (inputTick > predictTickCount + maxServerFrameIdx)
            {
                return;
            }

            var playerInput = new PlayerInput()
            {
                mousePos = InputManager.mousePos,
                inputUV = InputManager.inputUV,
                isInputFire = InputManager.isInputFire,
                skillId = InputManager.skillId,
                isSpeedUp = InputManager.isSpeedUp
            };

            netClient.Send(new Msg_PlayerInput()
            {
                input = playerInput,
                tick = inputTick,
            });

            tick2SendTimer[inputTick] = Time.realtimeSinceStartup;
            inputTick++;
        }

        private void StartGame(int mapId, PlayerServerInfo[] playerInfos, int localPlayerId)
        {
            hasStart = true;
            curMapId = mapId;

            var txt = Resources.Load<TextAsset>("Maps/" + curMapId + ".navmesh");
            NavMeshManager.DoInit(txt.text);
            maxBspDepth = BspTree.maxDepth;
            maxBspDepthNodeId = BspTree.maxDepthNodeId;

            this.playerCount = playerInfos.Length;
            this.PlayerServerInfos = playerInfos;
            this.localPlayerId = localPlayerId;
            Debug.TraceSavePath = _traceLogPath;
            allPlayers.Clear();
            for (int i = 0; i < playerCount; i++)
            {
                Debug.Trace("CreatePlayer");
                allPlayers.Add(new Player() { localId = i });
            }

            for (int i = 0; i < playerCount; i++)
            {
                var playerInfo = playerInfos[i];
                var prefab = ResourceManager.LoadPrefab(playerInfo.PrefabId);
                CollisionManager.Instance.RegisterPrefab(prefab, (int)EColliderLayer.Hero);
                HeroManager.InstantiateEntity(allPlayers[i], playerInfo.PrefabId, playerInfo.initPos);
            }
        }

        private void Step()
        {
            UpdateFrameInput();
            if (IsReplay)
            {
                if (curFrameIdx < frames.Count)
                {
                    Replay(curFrameIdx);
                    curFrameIdx++;
                }
                else
                {
                    Recoder();
                    netClient.Send(new Msg_HashCode()
                    {
                        tick = curFrameIdx,
                        hash = GetHash()
                    });
                    //TOdo 记录当前所有帧状态
                    // TraceHelper.TraceFrameState();

                    curFrameIdx++;
                }
            }
        }

        private void Recoder()
        {
            _Update();
        }

        private void _Update()
        {
            var deltaTime = new LFloat(true, 30);
            collisionManager.DoUpdate(deltaTime);
            DoUpdate(deltaTime);
            foreach (var mgr in _mgrs)
            {
                mgr.DoUpdate(deltaTime);
            }
        }

        public override void DoUpdate(LFloat deltaTime)
        {
        }

        private int GetHash()
        {
            int hash = 1;
            int idx = 0;
            foreach (var entity in allPlayers)
            {
                hash += entity.currentHealth.GetHash() * PrimerLUT.GetPrimer(idx++);
                hash += entity.currentHealth.GetHash() * PrimerLUT.GetPrimer(idx++);
            }

            foreach (var entity in EnemyManager.Instance.allEnemy)
            {
                hash += entity.currentHealth.GetHash() * PrimerLUT.GetPrimer(idx++);
                hash += entity.transform.GetHash() * PrimerLUT.GetPrimer(idx++);
            }

            return hash;
        }

        public static void StartGame(Msg_StartGame msg)
        {
            UnityEngine.Debug.Log("StartGame");
            Instance.StartGame(msg.mapId, msg.playerInfos, msg.localPlayerId);
        }

        public static void PushFrameInput(FrameInput input)
        {
            var frames = Instance.frames;
            for (int i = frames.Count; i <= input.tick; i++)
            {
                frames.Add(new FrameInput());
            }

            if (frames.Count == 0)
            {
                Instance.remainTime = 0;
            }

            maxServerFrameIdx = Math.Max(maxServerFrameIdx, input.tick);
            if (Instance.tick2SendTimer.TryGetValue(input.tick, out var val))
            {
                delays.Add(Time.realtimeSinceStartup - val);
            }

            frames[input.tick] = input;
        }

        private FrameInput GetFrame(int tick)
        {
            if (frames.Count > tick)
            {
                var frame = frames[tick];
                if (frame != null && frame.tick == tick)
                {
                    return frame;
                }
            }

            return null;
        }

        private void UpdateFrameInput()
        {
            CurFrameInput = GetFrame(curFrameIdx);
            var frame = CurFrameInput;
            for (int i = 0; i < playerCount; i++)
            {
                allPlayers[i].InputAgent = frame.inputs[i];
            }
        }

        private void Replay(int frameIdx)
        {
            _Update();
        }

        public override void DoAwake()
        {
            Instance = this;
            EnemyManager.maxCount = maxEnemyCount;
            var mgrs = GetComponents<BaseManager>();
            foreach (var mgr in mgrs)
            {
                if (mgr != null)
                {
                    RegisterManagers(mgr);
                }
            }
#if UNITY_EDITOR
            BspTree._debugMat = debugMat;
            BspTree._debugTrans = transform;
#endif
            //collision init
            collisionManager.DoAwake();
        }


        public override void DoStart()
        {
            InputManager.IsReplay = IsReplay;
            if (IsReplay)
            {
                //TODO 反序列化数据   
                // RecordHelper.Deserialize(recordFilePath, this);
            }
        }

        private void OnDestroy()
        {
            netClient?.Send(new Msg_QuitRoom());
            foreach (var mgr in _mgrs)
            {
                mgr.DoDestroy();
            }

            if (!IsReplay)
            {
                //TODO 退出时候序列化数据
                // RecordHelper.Serialize(recordFilePath, this);
            }

            Debug.FlushTrace();
            DoDestroy();
        }

        public override void DoDestroy()
        {
            //DumpPathFindReqs();
        }
    }
}