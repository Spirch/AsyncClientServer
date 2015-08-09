using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace AsyncClientServer
{
	public sealed class Client
	{
		internal string address;
		internal int port;

		internal bool isServerSocket;

		internal int id;
		internal bool closed;
		internal Socket socket;

		internal readonly byte[] socketBuffer;
		internal readonly List<byte> outBuffer;
		internal KindMessage KindOfMessage;
		internal int MessageLength;

		internal readonly Queue<byte[]> sendMsg;

		internal ManualResetEvent mreInit;
		internal ManualResetEvent mreMonitorDisconnect;
		internal ManualResetEvent mreIsConnected;

		internal ManualResetEvent mreBeginConnect;
		public delegate void ConnectedHandler(Client client);
		public event ConnectedHandler Connected;
		internal void RaiseConnected()
		{
			var handler = Connected;
			if (handler != null)
			{
				handler(this);
			}
		}

		internal ManualResetEvent mreBeginReceive;
		public delegate void MessageReceivedHandler(Client client, byte[] msg, KindMessage kindOfSend);
		public event MessageReceivedHandler MessageReceived;
		internal void RaiseMessageReceived(byte[] msg, KindMessage kindOfSend)
		{
			if (KindOfMessage == KindMessage.Message)
			{
				var handler = MessageReceived;
				if (handler != null)
				{
					handler(this, msg, kindOfSend);
				}
			}
			else if (KindOfMessage == KindMessage.ListClientId)
			{
				if(isServerSocket)
				{
					var handler = MessageReceived;
					if (handler != null)
					{
						handler(this, msg, kindOfSend);
					}
				}
				else
				{
					this.RaiseReceivedListClientId(msg.ToListOfInt());
				}
			}
			else if (KindOfMessage == KindMessage.ServerReady)
			{

			}
			else if (KindOfMessage == KindMessage.ClientId)
			{
				this.RaiseReceivedClientId(msg.ToInt());
			}
		}

		internal ManualResetEvent mreBeginSend;
		internal ManualResetEvent mreEndSend;
		public delegate void MessageSentHandler(Client client, int size);
		public event MessageSentHandler MessageSent;
		internal void RaiseMessageSent(int size)
		{
			var handler = MessageSent;
			if (handler != null)
			{
				handler(this, size);
			}
		}

		internal ManualResetEvent mreBeginDisconnect;
		public delegate void DisconnectedHandler(Client client);
		public event DisconnectedHandler Disconnected;
		internal void RaiseDisconnected()
		{
			var handler = Disconnected;
			if (handler != null)
			{
				handler(this);
			}
		}

		public delegate void SocketErrorHandler(Client client, Exception e);
		public event SocketErrorHandler SocketError;
		internal void RaiseSocketError(Exception e)
		{
			var handler = SocketError;
			if (handler != null)
			{
				handler(this, e);
			}
		}

		public delegate void ReceivedClientIdHandler(Client client);
		public event ReceivedClientIdHandler ReceivedClientId;
		internal void RaiseReceivedClientId(int Id)
		{
			this.id = Id;

			var handler = ReceivedClientId;
			if (handler != null)
			{
				handler(this);
			}
		}

		public delegate void ReceivedListClientIdHandler(Client client, IEnumerable<int> ids);
		public event ReceivedListClientIdHandler ReceivedListClientId;
		internal void RaiseReceivedListClientId(IEnumerable<int> ids)
		{
			var handler = ReceivedListClientId;
			if (handler != null)
			{
				handler(this, ids);
			}
		}

		public Client()
			: this(-1, false)
		{ }

		internal Client(int ClientId, bool serverSocket)
		{
			socketBuffer = new byte[Const.BufferSize];
			outBuffer = new List<byte>(Const.BufferSize);
			sendMsg = new Queue<byte[]>();
			mreIsConnected = new ManualResetEvent(true);

			isServerSocket = serverSocket;
			closed = true;
			id = ClientId;
		}

		public int Id { get { return id; } }

		public bool IsConnected()
		{
			return this.IsClientConnected();
		}

		public void Connect(string address, int port)
		{
			this.InitClient(address, port);
		}
		public void Close()
		{
			this.Disconnect();
		}
		
		public void Send(string message)
		{
			var msg = Encoding.UTF8.GetBytes(message);

			this.SendBytes(msg, KindMessage.Message);
		}

		public void Send(byte[] msg)
		{
			this.SendBytes(msg, KindMessage.Message);
		}

		internal void Send(byte[] msg, KindMessage kind)
		{
			this.SendBytes(msg, kind);
		}

		public void RequestListOfConnectedCliendId()
		{
			this.SendBytes(null, KindMessage.ListClientId);
		}
	}
}