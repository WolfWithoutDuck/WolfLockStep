using Lockstep.Network;

namespace Lockstep.FakeServer
{
    public class NetClient : IMessageDispatcher
    {
        private NetOuterProxy net = new NetOuterProxy();
        public Session Session;

        private int count = 0;
        public int id;

        public void Start()
        {
            net.Awake(NetworkProtocol.TCP);
            net.MessageDispatcher = this;
            net.MessagePacker = MessagePacker.Instance;
            Session = net.Create(Server.serverIpPoint);
            Session.Start();
        }

        public void Dispatch(Session session, Packet packet)
        {
            ushort opcode = packet.Opcode();
            var message = session.Network.MessagePacker.DeserializeFrom(opcode, packet.Bytes, Packet.Index,
                packet.Length - Packet.Index) as IMessage;

            var type = (EMsgType)opcode;
            switch (type)
            {
                case EMsgType.FrameInput:
                    OnFrameInput(session, message);
                    break;
                case EMsgType.StartGame:
                    OnStartGmae(session, message);
                    break;
            }
        }

        public void OnFrameInput(Session session, IMessage message)
        {
            var msg = message as Msg_FrameInput;
            //TODO GameManager
        }

        public void OnStartGmae(Session session, IMessage message)
        {
            var msg = message as Msg_StartGame;
            //TODO GameManager
        }

        public void Send(IMessage msg)
        {
            Session.Send(msg);
        }
    }
}