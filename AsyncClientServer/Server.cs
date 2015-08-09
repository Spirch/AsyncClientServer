using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

namespace AsyncClientServer
{
	public sealed class Server
	{
		private Socket listener;
		private Stack<int> nextClientId;
		private int maxid;
		private bool isServerRunning;
		private Dictionary<int, Client> clients;

		private ManualResetEvent mreBeginAccept;

		public delegate void ConnectedHandler(int ClientId);
		public event ConnectedHandler Connected;
		internal void RaiseConnected(int ClientId)
		{
			var handler = Connected;
			if (handler != null)
			{
				handler(ClientId);
			}
		}

		public delegate void MessageReceivedHandler(int ClientId, byte[] msg, KindMessage kindOfSend);
		public event MessageReceivedHandler MessageReceived;
		internal void RaiseMessageReceived(int ClientId, byte[] msg, KindMessage kindOfSend)
		{
			if (kindOfSend == KindMessage.Message)
			{
				var handler = MessageReceived;
				if (handler != null)
				{
					handler(ClientId, msg, kindOfSend);
				}
			}
			else if (kindOfSend == KindMessage.ListClientId)
			{
				GetClient(ClientId).SendBytes(clients.Keys.ToArrayOfByte(),KindMessage.ListClientId);
			}
		}

		public delegate void MessageSentHandler(int ClientId);
		public event MessageSentHandler MessageSent;
		internal void RaiseMessageSent(int ClientId)
		{
			var handler = MessageSent;
			if (handler != null)
			{
				handler(ClientId);
			}
		}

		public delegate void DisconnectedHandler(int ClientId);
		public event DisconnectedHandler Disconnected;
		internal void RaiseDisconnected(int ClientId)
		{
			var handler = Disconnected;
			if (handler != null)
			{
				handler(ClientId);
			}
		}

		public delegate void SocketErrorHandler(Client client, Exception e);
		public event SocketErrorHandler SocketError;
		internal void RaiseSocketError(Client client, Exception e)
		{
			var handler = SocketError;
			if (handler != null)
			{
				handler(client, e);
			}
		}

		public Server()
		{
			isServerRunning = false;
			clients = new Dictionary<int, Client>();
			nextClientId = new Stack<int>();
			nextClientId.Push(maxid);
		}

		public void StopServer()
		{
			if (isServerRunning)
			{
				isServerRunning = false;
				mreBeginAccept.SetIfNotNull();
				CloseAll();
			}
		}

		public void StartServer(string address, int port)
		{
			if (!isServerRunning)
			{
				isServerRunning = true;

				var ip = new IPAddress(address.Split('.').Select(x => Convert.ToByte(x)).ToArray());
				var endpoint = new IPEndPoint(ip, port);

				listener = MiscOperation.NewSocket();

				listener.Bind(endpoint);
				listener.Listen(Const.BackLogLimit);

				new Thread(this.BeginAccept).Start();
			}
		}

		private void BeginAccept()
		{
			using(mreBeginAccept = new ManualResetEvent(false))
			{
				while (isServerRunning)
				{
					mreBeginAccept.Reset();
					listener.BeginAccept(EndAccept, listener);
					mreBeginAccept.WaitOne();
				}
			}

			listener.Close();
			listener.Dispose();
			listener = null;
		}

		private void EndAccept(IAsyncResult result)
		{
			if (!isServerRunning)
				return;

			var state = (Socket)result.AsyncState;

			var listener = state.EndAccept(result);

			HandleNewClient(listener);

			mreBeginAccept.SetIfNotNull();
		}

		private void HandleNewClient(Socket socket)
		{
			Client client;

			lock (clients)
			lock (nextClientId)
			{
				var id = nextClientId.Pop();

				if (nextClientId.Count == 0)
				{
					nextClientId.Push(Interlocked.Increment(ref maxid));
				}

				client = new Client(id, true);
				clients.Add(id, client);
			}

			client.Connected += state_Connected;
			client.SocketError += client_SocketError;
			client.MessageReceived += state_MessageReceived;
			client.Disconnected += state_Disconnected;
			client.InitServer(socket);
		}

		private void state_Connected(Client client)
		{
			client.SendBytes(null, KindMessage.ServerReady);
			client.SendBytes(client.id.ToByte(), KindMessage.ClientId);
			client.SendBytes(clients.Keys.ToArrayOfByte(), KindMessage.ListClientId);

			RaiseConnected(client.Id);
		}

		private void state_Disconnected(Client client)
		{
			RemoveClient(client);
		}

		private void RemoveClient(Client client)
		{
			client.Connected -= state_Connected;
			client.SocketError -= client_SocketError;
			client.Disconnected -= state_Disconnected;
			client.MessageReceived -= state_MessageReceived;

			Close(client);

			lock (clients)
			lock (nextClientId)
			{
				clients.Remove(client.Id);
				nextClientId.Push(client.Id);
			}

			RaiseDisconnected(client.Id);
		}

		private void client_SocketError(Client client, Exception e)
		{
			RaiseSocketError(client, e);
		}

		private void client_MessageSent(int ClientId)
		{
			RaiseMessageSent(ClientId);
		}

		private void state_MessageReceived(Client client, byte[] msg, KindMessage kindOfSend)
		{
			RaiseMessageReceived(client.Id, msg, kindOfSend);
		}

		private Client GetClient(int id)
		{
			Client state;

			return clients.TryGetValue(id, out state) ? state : null;
		}

		public void SendAll(string message)
		{
			var keys = clients.Keys.OrderByDescending(o => o);
			var msg = Encoding.UTF8.GetBytes(message);

			foreach (var key in keys)
			{
				Send(key, msg);
			}
		}

		public void Send(int id, string message)
		{
			var msg = Encoding.UTF8.GetBytes(message);

			Send(id, msg);
		}

		private void Send(int id, byte[] message)
		{
			var client = GetClient(id);

			if (client != null)
			{
				client.Send(message);
			}
		}

		public void CloseAll()
		{
			var keys = clients.Keys.OrderByDescending(o => o);

			foreach (var key in keys)
			{
				Close(key);
			}
		}

		public void Close(int id)
		{
			var client = GetClient(id);

			Close(client);
		}

		private void Close(Client client)
		{
			if (client != null)
			{
				client.Close();
			}
		}
	}
}