using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncClientServer
{
	public static class ConnectOperation
	{
		internal static bool IsClientConnected(this Client client)
		{
			bool connected = false;
			bool connected1 = false;
			bool connected2 = false;
			bool connected3 = false;
			bool poll1 = false;
			bool poll2 = false;

			client.mreIsConnected.WaitOne();
			client.mreIsConnected.Reset();

			try
			{
				if (!client.closed && client.socket != null)
				{
					lock (client.socket)
					{
						connected1 = !client.closed && client.socket != null;
						poll1 = client.socket.Poll(1000, SelectMode.SelectRead);
						poll2 = client.socket.Available == 0;
						connected2 = !(poll1 && poll2);
						connected3 = client.socket.Connected;

						connected = connected1 && connected2 && connected3;
					}
				}
			}
			catch (Exception e)
			{
				client.HandleError(e);
				connected = false;
			}

			client.mreIsConnected.SetIfNotNull();

			return connected;
		}

		internal static void BeginConnect(this Client client)
		{
			client.closed = true;
			client.socket = MiscOperation.NewSocket();

			try
			{
				client.socket.BeginConnect(client.address, client.port, EndConnect, client);
			}
			catch (Exception e)
			{
				client.HandleError(e);
			}
		}

		private static void EndConnect(IAsyncResult result)
		{
			var client = (Client)result.AsyncState;

			try
			{
				client.socket.EndConnect(result);

				client.closed = false;
			}
			catch (Exception e)
			{
				client.HandleError(e);
			}

			client.mreBeginConnect.SetIfNotNull();
		}
	}
}
