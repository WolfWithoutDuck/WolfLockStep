using System.Collections.Generic;
using Lockstep.Network;

namespace Lockstep.FakeServer
{
    public class Room
    {
        public bool isRunning;
        public const int maxPlayerCount = 2;
        public PlayerServerInfo[] playerInfos;
        public Session[] PlayerSessions;
        public int curCount = 0;
        public Dictionary<int, int> id2LocalId = new Dictionary<int, int>();

        public void Init(int type)
        {
            playerInfos = new PlayerServerInfo[maxPlayerCount];
            PlayerSessions = new Session[maxPlayerCount];
        }

        private int curTick;

        public void DoUpdate(float timeSinceStartUp, float deltaTime)
        {
            if (!isRunning)
                return;
            if (tick2Inputs.TryGetValue(curTick, out var inputs))
            {
                if (inputs != null)
                {
                    bool isFullInput = true;
                    for (int i = 0; i < inputs.Length; i++)
                    {
                        if (inputs[i] == null)
                        {
                            isFullInput = false;
                            break;
                        }
                    }

                    if (isFullInput)
                    {
                        BoardInputMsg(curTick, inputs);
                        tick2Inputs.Remove(curTick);
                        curTick++;
                    }
                }
            }
        }


        public void StartGame()
        {
            isRunning = true;
            curTick = 0;
            var frame = new Msg_StartGame();
            frame.mapId = 0;
            frame.playerInfos = playerInfos;
            for (int i = 0; i < maxPlayerCount; i++)
            {
                var session = PlayerSessions[i];
                frame.localPlayerId = i;
                var bytes = frame.ToBytes();
                session.Send((int)EMsgType.StartGame, bytes);
            }
        }


        public Dictionary<int, PlayerInput[]> tick2Inputs = new Dictionary<int, PlayerInput[]>();
        public int curLocalId;

        public void Join(Session session, PlayerServerInfo player)
        {
            if (id2LocalId.ContainsKey(player.Id))
                return;
            id2LocalId[player.Id] = curLocalId;
            playerInfos[curLocalId] = player;
            PlayerSessions[curLocalId] = session;
            curLocalId++;
        }

        public void BoardInputMsg(int tick, PlayerInput[] inputs)
        {
            var frame = new Msg_FrameInput();
            frame.input = new FrameInput()
            {
                tick = tick,
                inputs = inputs
            };
            var bytes = frame.ToBytes();
            for (int i = 0; i < maxPlayerCount; i++)
            {
                var session = PlayerSessions[i];
                session.Send((int)EMsgType.FrameInput, bytes);
            }
        }

        public void OnPlayerInput(int useId, Msg_PlayerInput msg)
        {
            int localId = 0;
            if (!id2LocalId.TryGetValue(useId, out localId))
                return;
            PlayerInput[] val;
            if (!tick2Inputs.TryGetValue(msg.tick, out val))
            {
                val = new PlayerInput[maxPlayerCount];
            }

            val[localId] = msg.input;
        }
    }
}