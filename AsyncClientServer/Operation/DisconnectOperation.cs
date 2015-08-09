using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncClientServer
{
    internal static class DisconnectOperation
    {
        internal static void Disconnect(this Client client)
        {
            try
            {
                if (!client.closed)
                {
                    client.closed = true;

                    client.mreIsConnected.SetIfNotNull();
                    client.mreBeginReceive.SetIfNotNull();
                    client.mreBeginSend.SetIfNotNull();
                    client.mreEndSend.SetIfNotNull();
                    client.mreMonitorDisconnect.SetIfNotNull();

                    if (client.socket != null)
                    {
                        lock (client.socket)
                        {
                            if (client.socket != null)
                            {
                                client.socket.Shutdown(SocketShutdown.Both);
                                BeginDisconnect(client);

                                client.socket.Close();
                                client.socket.Dispose();
                                client.socket = null;

                                client.RaiseDisconnected();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                client.HandleError(e);
            }
        }

        internal static void MonitorDisconnect(this Client client)
        {
            using(client.mreMonitorDisconnect = new ManualResetEvent(false))
            {
                while (!client.closed)
                {
                    if (!client.IsClientConnected())
                    {
                        Disconnect(client);
                    }
                    client.mreInit.SetIfNotNull();
                    client.mreMonitorDisconnect.WaitOne(Const.MonitorDisconnectCycle);
                    client.mreIsConnected.WaitOne();
                }
            }
        }

        private static void BeginDisconnect(Client client)
        {
            using(client.mreBeginDisconnect = new ManualResetEvent(false))
            {
                try
                {
                    client.socket.BeginDisconnect(false, EndDisconnect, client);
                }
                catch (Exception e)
                {
                    client.HandleError(e);
                }

                client.mreBeginDisconnect.WaitOne(5000);
            }
        }

        private static void EndDisconnect(IAsyncResult result)
        {
            var client = (Client)result.AsyncState;

            try
            {
                client.socket.EndDisconnect(result);
            }
            catch (Exception e)
            {
                client.HandleError(e);
            }

            client.mreBeginDisconnect.SetIfNotNull();
        }
    }
}
