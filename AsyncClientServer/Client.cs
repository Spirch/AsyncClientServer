using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace AsyncClientServer
{
	public class Client : IDisposable, AsyncClientServer.IClient
	{
		private readonly object myLock = new object();

		private readonly bool isServerSocket;
		internal readonly ConcurrentQueue<byte[]> sendMessage;

		internal bool closed { get; private set; }
		private Socket socket;
		private string address;
		private int port;

		public int Id { get; private set; }

		public Client()
		{
			sendMessage = new ConcurrentQueue<byte[]>();
			mreIsConnected = new ManualResetEvent(true);

			isServerSocket = false;
			closed = true;
			Id = -1;
		}

		internal Client(int ClientId, Socket socket)
		{
			sendMessage = new ConcurrentQueue<byte[]>();
			mreIsConnected = new ManualResetEvent(true);

			isServerSocket = true;
			closed = false;
			Id = ClientId;
			this.socket = socket;
		}

		private ManualResetEvent mreInit;
		internal void InitSocket()
		{
			using (mreInit = new ManualResetEvent(false))
			{
				mreInit.Reset();
				new Thread(BeginReceive).Start();
				mreInit.WaitOne();

				mreInit.Reset();
				new Thread(BeginSend).Start();
				mreInit.WaitOne();

				mreInit.Reset();
				new Thread(MonitorDisconnect).Start();
				mreInit.WaitOne();
			}
		}

		public bool IsConnected()
		{
			return this.IsClientConnected();
		}

		public void Connect(string address, int port)
		{
			if (socket == null)
			{
				Id = -1;
				this.address = address;
				this.port = port;

				using (mreBeginConnect = new ManualResetEvent(false))
				{
					mreBeginConnect.Reset();
					BeginConnect();
					mreBeginConnect.WaitOne();
				}

				if (!closed)
				{
					InitSocket();

					OnConnected();
				}
				else
				{
					OnDisconnected();
				}
			}
		}

		public void Close()
		{
			Disconnect();
		}

		private void Disconnect()
		{
			try
			{
				lock (myLock)
				{
					if (!closed)
					{
						closed = true;

						mreIsConnected.SetIfNotNull();
						mreBeginReceive.SetIfNotNull();
						mreBeginSend.SetIfNotNull();
						mreEndSend.SetIfNotNull();
						mreMonitorDisconnect.SetIfNotNull();

						if (socket != null)
						{
							socket.Shutdown(SocketShutdown.Both);
							BeginDisconnect();

							socket.Close();
							socket.Dispose();
							socket = null;

							OnDisconnected();
						}
					}
				}
			}
			catch (Exception e)
			{
				this.HandleError(e);
			}
		}

		public void RequestListOfConnectedCliendId()
		{
			SendBytes(null, MessageKind.ListClientId);
		}

		public void Send(string message)
		{
			var msg = Encoding.UTF8.GetBytes(message);

			SendBytes(msg, MessageKind.Message);
		}

        public void Send(byte[] message)
		{
            SendBytes(message, MessageKind.Message);
		}

        internal void Send(byte[] message, MessageKind kind)
		{
            SendBytes(message, kind);
		}

        private void SendBytes(byte[] message, MessageKind kind)
		{
			try
			{
                if (message == null)
				{
                    message = new byte[0];
				}

                var length = message.Length;

				if (length > ushort.MaxValue)
				{
					throw new ArgumentOutOfRangeException();
				}

                byte[] outMessage;
                EnvelopeSend(message, kind, length, out outMessage);

                sendMessage.Enqueue(outMessage);

				mreBeginSend.SetIfNotNull();
			}
			catch (Exception e)
			{
				this.HandleError(e);
			}
		}

		private void EnvelopeSend(byte[] response, MessageKind kindOfSend, int length, out byte[] outMessage)
		{
			outMessage = new byte[length + Const.TotalSizeOfEnvelope];

			outMessage[0] = (byte)kindOfSend;
			Array.Copy(length.ToByte(), 0, outMessage, Const.SizeOfEnvelopeKind, Const.SizeOfEnvelopeLength);
			Array.Copy(response, 0, outMessage, Const.TotalSizeOfEnvelope, length);
		}

		public event SocketEventHandler<Client, ClientIdEventArgs> ClientIdReceived = delegate { };
		internal void OnClientIdReceived(int id)
		{
			Id = id;
			ClientIdReceived(this, ClientIdEventArgs.NewEvent(id));
		}

		public event SocketEventHandler<Client, ListClientIdEventArgs> ListClientIdReceived = delegate { };
		internal void OnListClientIdReceived(IEnumerable<int> id)
		{
			ListClientIdReceived(this, ListClientIdEventArgs.NewEvent(id));
		}

		public event SocketEventHandler<Client, EventArgs> Connected = delegate { };
		internal void OnConnected()
		{
			Connected(this, EventArgs.Empty);
		}

		public event SocketEventHandler<Client, EventArgs> Disconnected = delegate { };
		internal void OnDisconnected()
		{
			Disconnected(this, EventArgs.Empty);
		}

		public event SocketEventHandler<Client, SocketErrorEventArgs> SocketError = delegate { };
		internal void OnSocketError(Exception e)
		{
			SocketError(this, SocketErrorEventArgs.NewEvent(e));
		}

		public event SocketEventHandler<Client, EventArgs> MessageSent = delegate { };
		internal void OnMessageSent()
		{
			MessageSent(this, EventArgs.Empty);
		}

		public event SocketEventHandler<Client, ReceivedEventArgs> MessageReceived = delegate { };
		internal void OnMessageReceived(byte[] message, MessageKind messageKind)
		{
			if (messageKind == MessageKind.Message)
			{
				MessageReceived(this, ReceivedEventArgs.NewEvent(message, messageKind));
			}
			else if (messageKind == MessageKind.ListClientId)
			{
				if (isServerSocket)
				{
					MessageReceived(this, ReceivedEventArgs.NewEvent(message, messageKind));
				}
				else
				{
					OnListClientIdReceived(message.ToListOfInt());
				}
			}
			else if (messageKind == MessageKind.ServerReady)
			{

			}
			else if (messageKind == MessageKind.ClientId)
			{
				OnClientIdReceived(message.ToInt());
			}
		}

		private ManualResetEvent mreIsConnected;
		private bool IsClientConnected()
		{
			//this code is verbose because i had issue at some point, it should be done in one line but i will keep it verbose for now.
			bool connected = false;
			bool connected1 = false;
			bool connected2 = false;
			bool connected3 = false;
			bool poll1 = false;
			bool poll2 = false;

			mreIsConnected.WaitOne();
			mreIsConnected.Reset();

			try
			{
				lock (myLock)
				{
					connected1 = !closed && socket != null;
					poll1 = connected1 && socket.Poll(1000, SelectMode.SelectRead);
					poll2 = connected1 && socket.Available == 0;
					connected2 = !(poll1 && poll2);
					connected3 = connected1 && socket.Connected;

					connected = connected1 && connected2 && connected3;
				}
			}
			catch (Exception e)
			{
				this.HandleError(e);
				connected = false;
			}

			mreIsConnected.SetIfNotNull();

			return connected;
		}

		private ManualResetEvent mreBeginConnect;
		private  void BeginConnect()
		{
			closed = true;
			socket = MiscOperation.NewSocket();

			try
			{
				socket.BeginConnect(address, port, EndConnect, null);
			}
			catch (Exception e)
			{
				this.HandleError(e);
			}
		}

		private void EndConnect(IAsyncResult result)
		{
			try
			{
				socket.EndConnect(result);

				closed = false;
			}
			catch (Exception e)
			{
				this.HandleError(e);
			}

			mreBeginConnect.SetIfNotNull();
		}

		private ManualResetEvent mreMonitorDisconnect;
		private void MonitorDisconnect()
		{
			using (mreMonitorDisconnect = new ManualResetEvent(false))
			{
				while (!closed)
				{
					if (!IsClientConnected())
					{
						Disconnect();
					}
					mreInit.SetIfNotNull();
					mreMonitorDisconnect.WaitOne(Const.MonitorDisconnectCycle);
					mreIsConnected.WaitOne();
				}
			}
		}

		private ManualResetEvent mreBeginDisconnect;
		private void BeginDisconnect()
		{
			using (mreBeginDisconnect = new ManualResetEvent(false))
			{
				try
				{
					socket.BeginDisconnect(false, EndDisconnect, null);
				}
				catch (Exception e)
				{
					this.HandleError(e);
				}

				mreBeginDisconnect.WaitOne(5000);
			}
		}

		private void EndDisconnect(IAsyncResult result)
		{
			try
			{
				socket.EndDisconnect(result);
			}
			catch (Exception e)
			{
				this.HandleError(e);
			}

			mreBeginDisconnect.SetIfNotNull();
		}

		private ManualResetEvent mreBeginReceive;
		private void BeginReceive()
		{
			using (mreBeginReceive = new ManualResetEvent(false))
			using (var buffer = new SocketBuffer())
			{
				try
				{
					while (!closed)
					{
						mreBeginReceive.Reset();
						socket.BeginReceive(buffer.socketBuffer, 0, Const.BufferSize, SocketFlags.None, EndReceive, buffer);
						mreInit.SetIfNotNull();
						mreBeginReceive.WaitOne();
						mreIsConnected.WaitOne();
					}
				}
				catch (Exception e)
				{
					this.HandleError(e);
				}
			}
		}

		private void EndReceive(IAsyncResult result)
		{
			try
			{
				lock (myLock)
				{
					if (closed)
					{
						return;
					}

					var receive = socket.EndReceive(result);
					var buffer = result.AsyncState as SocketBuffer;

					if (receive == 0 || buffer == null)
					{
						Disconnect();
						return;
					}

					ProcessNewData(receive, buffer);
					mreBeginReceive.SetIfNotNull();
				}
			}
			catch (Exception e)
			{
				this.HandleError(e);
			}
		}

		private void ProcessNewData(int receive, SocketBuffer buffer)
		{
			buffer.outBuffer.AddRange(buffer.socketBuffer.Take(receive));

			do
			{
				EnvelopeRead(buffer);

				if (buffer.outBuffer.Count >= buffer.messageLength)
				{
					var msg = buffer.outBuffer.GetRange(0, buffer.messageLength).ToArray();
					buffer.outBuffer.RemoveRange(0, buffer.messageLength);

					OnMessageReceived(msg, buffer.messageKind);

					buffer.messageKind = MessageKind.Unknown;
					buffer.messageLength = buffer.outBuffer.Count >= Const.TotalSizeOfEnvelope ? 0 : int.MaxValue;
				}
			} while (buffer.outBuffer.Count >= buffer.messageLength);
		}

		private void EnvelopeRead(SocketBuffer buffer)
		{
			if (buffer.messageKind == MessageKind.Unknown && buffer.outBuffer.Count >= Const.TotalSizeOfEnvelope)
			{
				buffer.messageKind = (MessageKind)buffer.outBuffer[0];

				if (!Enum.IsDefined(typeof(MessageKind), buffer.messageKind))
				{
					buffer.messageKind = MessageKind.Unknown;
					throw new FormatException("Doesn't understand the envelope!");
				}

				byte[] length = buffer.outBuffer.GetRange(Const.SizeOfEnvelopeKind, Const.SizeOfEnvelopeLength).ToArray();

				buffer.messageLength = length.ToInt();

				if (buffer.messageLength > ushort.MaxValue || buffer.messageLength < 0)
				{
					throw new ArgumentOutOfRangeException();
				}

				buffer.outBuffer.RemoveRange(0, Const.TotalSizeOfEnvelope);
			}
		}

		private ManualResetEvent mreBeginSend;
		private ManualResetEvent mreEndSend;
		private void BeginSend()
		{
			byte[] message;

			while (!sendMessage.IsEmpty)
			{
				sendMessage.TryDequeue(out message);
			}

			message = null;

			using (mreBeginSend = new ManualResetEvent(false))
			using (mreEndSend = new ManualResetEvent(false))
			{
				try
				{
					while (!closed)
					{
						mreBeginSend.Reset();

						if (sendMessage.TryDequeue(out message) && message != null)
						{
							mreEndSend.Reset();
							socket.BeginSend(message, 0, message.Length, SocketFlags.None, EndSend, null);
							mreEndSend.WaitOne();
							message = null;
						}

						mreInit.SetIfNotNull();
						mreBeginSend.WaitOne();
						mreIsConnected.WaitOne();
					}
				}
				catch (Exception e)
				{
					this.HandleError(e);
				}
			}
		}

		private void EndSend(IAsyncResult result)
		{
			try
			{
                if (closed)
                {
                    return;
                }

				int size = socket.EndSend(result);

				OnMessageSent();

				mreBeginSend.SetIfNotNull();
				mreEndSend.SetIfNotNull();
			}
			catch (Exception e)
			{
				this.HandleError(e);
			}
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}
