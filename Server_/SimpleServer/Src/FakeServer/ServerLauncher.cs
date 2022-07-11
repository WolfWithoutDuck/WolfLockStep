using System.Diagnostics;
using Lockstep.Logging;
using Lockstep.Network;
using Debug = Lockstep.Logging.Debug;


namespace Lockstep.FakeServer
{
    public class ServerLauncher
    {
        private static Server server;

        public static void Main()
        {
            OneThreadSynchronizationContext contex = new OneThreadSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(contex);
            Debug.Log("Mian start");
            try
            {
                DoAwake();
                while (true)
                {
                    try
                    {
                        Thread.Sleep(3);
                        contex.Update();
                        server.Update();
                    }
                    catch (ThreadAbortException e)
                    {
                        return;
                    }
                    catch (Exception e)
                    {
                        Log.Error(e.ToString());
                    }
                }
            }
            catch (ThreadAbortException e)
            {
                return;
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
        }

        private static void DoAwake()
        {
            server = new Server();
            server.start();
        }
    }
}