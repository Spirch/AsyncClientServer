using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncClientServer
{
	internal static class InitOperation
	{
		internal static void InitServer(this Client client, Socket listener)
		{
			if (client.socket == null)
			{
				client.address = null;
				client.port = 0;
				client.socket = listener;
				client.closed = false;
				
				client.InitFinal();
				
				client.RaiseConnected();
			}
		}

		internal static void InitClient(this Client client, string address, int port)
		{
			if (client.socket == null)
			{
				client.id = -1;
				client.address = address;
				client.port = port;

				using (client.mreBeginConnect = new ManualResetEvent(false))
				{
					client.mreBeginConnect.Reset();
					client.BeginConnect();
					client.mreBeginConnect.WaitOne();
				}

				if(!client.closed)
				{
					client.InitFinal();

					client.RaiseConnected();
				}
				else
				{
					client.RaiseDisconnected();
				}
			}
		}

		internal static void InitFinal(this Client client)
		{
			if (!client.closed)
			{
				using(client.mreInit = new ManualResetEvent(false))
				{
					client.mreInit.Reset();
					new Thread(client.BeginReceive).Start();
					client.mreInit.WaitOne();

					client.mreInit.Reset();
					new Thread(client.BeginSend).Start();
					client.mreInit.WaitOne();

					client.mreInit.Reset();
					new Thread(client.MonitorDisconnect).Start();
					client.mreInit.WaitOne();
				}
			}
		}
	}
}
