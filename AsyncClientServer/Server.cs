using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace AsyncClientServer
{
	public class Server : IDisposable, AsyncClientServer.IServer
	{
		private readonly ConcurrentStack<int> nextClientId;
		private readonly ConcurrentDictionary<int, Client> clients;

		private int maxid;
		private bool isServerRunning;
		private Socket listener;

		public Server()
		{
			isServerRunning = false;
			clients = new ConcurrentDictionary<int, Client>();
			nextClientId = new ConcurrentStack<int>();
		}

		public void Start(string address, int port)
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

		public void Stop()
		{
			if (isServerRunning)
			{
				isServerRunning = false;
				mreBeginAccept.SetIfNotNull();
				CloseAll();
			}
		}

		private Client GetClient(int id)
		{
			Client client;

			return clients.TryGetValue(id, out client) ? client : null;
		}

		private void RemoveClient(Client client)
		{
			client.Connected -= client_OnConnected;
			client.SocketError -= client_OnSocketError;
			client.Disconnected -= client_OnDisconnected;
			client.MessageReceived -= client_OnMessageReceived;

			Close(client);

			if (!clients.TryRemove(client.Id, out client))
			{
				throw new Exception("if (!clients.TryRemove(client.Id, out client)))");
			}

			nextClientId.Push(client.Id);

			OnDisconnected(client);
		}

		public void Close(int id)
		{
			var client = GetClient(id);

			Close(client);
		}

		public void CloseAll()
		{
			var keys = clients.Keys.OrderByDescending(o => o);

			foreach (var key in keys)
			{
				Close(key);
			}
		}

		private void Close(Client client)
		{
			if (client != null)
			{
				client.Close();
			}
		}

		public void Send(int id, string message)
		{
			var msg = Encoding.UTF8.GetBytes(message);

			Send(id, msg);
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

		private void Send(int id, byte[] message)
		{
			var client = GetClient(id);

			if (client != null)
			{
				client.Send(message);
			}
		}

		public event SocketEventHandler<Server, ServerEventArgs> Connected = delegate { };
		internal void OnConnected(Client client)
		{
			Connected(this, ServerEventArgs.NewEvent(client));
		}

		public event SocketEventHandler<Server, ServerEventArgs> Disconnected = delegate { };
		internal void OnDisconnected(Client client)
		{
			Disconnected(this, ServerEventArgs.NewEvent(client));
		}

		public event SocketEventHandler<Server, ServerSocketErrorEventArgs> SocketError = delegate { };
		internal void OnSocketError(Client client, Exception exception)
		{
			SocketError(this, ServerSocketErrorEventArgs.NewEvent(client, exception));
		}

		public event SocketEventHandler<Server, ServerEventArgs> MessageSent = delegate { };
		internal void OnMessageSent(Client client)
		{
			MessageSent(this, ServerEventArgs.NewEvent(client));
		}

		public event SocketEventHandler<Server, ServerReceivedEventArgs> MessageReceived = delegate { };
		internal void OnMessageReceived(Client client, byte[] message, MessageKind messageKind)
		{
			if (messageKind == MessageKind.Message)
			{
				MessageReceived(this, ServerReceivedEventArgs.NewEvent(client, message, messageKind));
			}
			else if (messageKind == MessageKind.ListClientId)
			{
				client.Send(clients.Keys.ToArrayOfByte(), MessageKind.ListClientId);
			}
		}

		private void client_OnConnected(Client client, EventArgs eventArgs)
		{
			client.Send(null, MessageKind.ServerReady);
			client.Send(client.Id.ToByte(), MessageKind.ClientId);
			client.Send(clients.Keys.ToArrayOfByte(), MessageKind.ListClientId);

			OnConnected(client);
		}

		private void client_OnDisconnected(Client client, EventArgs eventArgs)
		{
			RemoveClient(client);
		}

		private void client_OnSocketError(Client client, SocketErrorEventArgs eventArgs)
		{
			OnSocketError(client, eventArgs.Exception);
		}

		private void client_OnMessageSent(Client client)
		{
			OnMessageSent(client);
		}

		private void client_OnMessageReceived(Client client, ReceivedEventArgs eventArgs)
		{
			OnMessageReceived(client, eventArgs.Message, eventArgs.MessageKind);
		}

		private ManualResetEvent mreBeginAccept;
		private void BeginAccept()
		{
			using (mreBeginAccept = new ManualResetEvent(false))
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
			int id;

			if (!nextClientId.TryPop(out id))
			{
				id = Interlocked.Increment(ref maxid);
			}

			var client = new Client(id, socket);

			if (!clients.TryAdd(id, client))
			{
				throw new Exception("if(!clients.TryAdd(id, client))");
			}

			client.Connected += client_OnConnected;
			client.SocketError += client_OnSocketError;
			client.MessageReceived += client_OnMessageReceived;
			client.Disconnected += client_OnDisconnected;

			client.InitSocket();
			client.OnConnected();
		}
		
		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}
