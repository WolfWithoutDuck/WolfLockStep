using System.Diagnostics;
using Lockstep.Network;
using Lockstep.Logging;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.FakeServer;

public class Room
{
    public bool isRunning;
    public const int maxPlayerCount = 2;
    public PlayerServerInfo[] playerInfos;
    public Session[] PlayerSessions;
    public int curCount = 0;
    public Dictionary<int, int> id2LocalId = new Dictionary<int, int>();

    public Dictionary<int, PlayerInput[]> tick2Inputs = new Dictionary<int, PlayerInput[]>();
    public Dictionary<int, int[]> tick2Hashes = new Dictionary<int, int[]>();
    public int curLocalId;


    public void Init(int type)
    {
        playerInfos = new PlayerServerInfo[maxPlayerCount];
        PlayerSessions = new Session[maxPlayerCount];
    }

    private int curTick;

    public void StartGame()
    {
        isRunning = false;
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

    public void DoUpdate(float timeSinceStartUp, float deltaTime)
    {
        if (!isRunning)
        {
            return;
        }

        CheckInput();
    }

    private void CheckInput()
    {
        if (tick2Inputs.TryGetValue(curTick, out var inputs))
        {
            if (inputs != null)
            {
                bool isFullInpus = true;
                for (int i = 0; i < inputs.Length; i++)
                {
                    if (inputs[i] == null)
                    {
                        isFullInpus = false;
                        break;
                    }
                }

                if (isFullInpus)
                {
                    BoardInputMsg(curTick, inputs);
                    tick2Inputs.Remove(curTick);
                    curTick++;
                }
            }
        }
    }

    private void BoardInputMsg(int tick, PlayerInput[] inputs)
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
            var sessions = PlayerSessions[i];
            sessions.Send((int)EMsgType.FrameInput, bytes);
        }
    }

    public void OnPlayerInput(int useId, Msg_PlayerInput msg)
    {
        int localId = 0;
        if (!id2LocalId.TryGetValue(useId, out localId))
        {
            return;
        }

        PlayerInput[] inputs;
        if (!tick2Inputs.TryGetValue(msg.tick, out inputs))
        {
            inputs = new PlayerInput[maxPlayerCount];
            tick2Inputs.Add(msg.tick, inputs);
        }

        inputs[localId] = msg.input;
        CheckInput();
    }

    public void OnPlayerHashCode(int userId, Msg_HashCode msg)
    {
        int localId = 0;
        if (!id2LocalId.TryGetValue(userId, out localId))
        {
            return;
        }

        int[] hashes;
        if (!tick2Hashes.TryGetValue(msg.tick, out hashes))
        {
            hashes = new int[maxPlayerCount];
            tick2Hashes.Add(msg.tick, hashes);
        }

        hashes[localId] = msg.hash;

        foreach (var hash in hashes)
        {
            if (hash == 0)
            {
                return;
            }
        }

        bool isSame = true;

        var val = hashes[0];

        foreach (var hash in hashes)
        {
            if (hash != val)
            {
                isSame = false;
                break;
            }
        }

        if (!isSame)
        {
            Debug.Log(msg.tick + " Hash is different " + val);
        }
    }

    public void Join(Session session, PlayerServerInfo player)
    {
        if (id2LocalId.ContainsKey(player.Id))
        {
            return;
        }

        id2LocalId[player.Id] = curLocalId;
        playerInfos[curLocalId] = player;
        PlayerSessions[curLocalId] = session;
        curLocalId++;
    }
}