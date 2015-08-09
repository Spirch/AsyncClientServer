using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncClientServer
{
	internal static class SendOperation
	{
		internal static void SendBytes(this Client client, byte[] msg, KindMessage kind)
		{
			try
			{
				if(msg == null)
				{
					msg = new byte[0];
				}

				var length = msg.Length;

				if (length > ushort.MaxValue)
				{
					throw new ArgumentOutOfRangeException();
				}

				byte[] outMsg;
				EnvelopeSend(msg, kind, length, out outMsg);

				lock (client.sendMsg)
				{
					client.sendMsg.Enqueue(outMsg);
				}

				client.mreBeginSend.SetIfNotNull();
			}
			catch (Exception e)
			{
				client.HandleError(e);
			}
		}

		internal static void EnvelopeSend(byte[] response, KindMessage kindOfSend, int length, out byte[] outMsg)
		{
			outMsg = new byte[length + Const.TotalSizeOfEnvelope];

			outMsg[0] = (byte)kindOfSend;
			Array.Copy(length.ToByte(), 0, outMsg, Const.SizeOfEnvelopeKind, Const.SizeOfEnvelopeLength);
			Array.Copy(response, 0, outMsg, Const.TotalSizeOfEnvelope, length);
		}

		internal static void BeginSend(this Client client)
		{
			byte[] msg = null;

			client.sendMsg.Clear();
			
			using(client.mreBeginSend = new ManualResetEvent(false))
			using(client.mreEndSend = new ManualResetEvent(false))
			{
				try
				{
					while (!client.closed)
					{
						client.mreBeginSend.Reset();
						if (client.sendMsg.Count > 0)
						{
							lock (client.sendMsg)
							{
								if (client.sendMsg.Count > 0)
								{
									msg = client.sendMsg.Dequeue();
								}
							}

							if (msg != null)
							{
								client.mreEndSend.Reset();
								client.socket.BeginSend(msg, 0, msg.Length, SocketFlags.None, EndSend, client);
								client.mreEndSend.WaitOne();
								msg = null;
							}
						}
						client.mreInit.SetIfNotNull();
						client.mreBeginSend.WaitOne();
						client.mreIsConnected.WaitOne();
					}
				}
				catch (Exception e)
				{
					client.HandleError(e);
				}
			}
		}

		private static void EndSend(IAsyncResult result)
		{
			var client = (Client)result.AsyncState;

			if (client.closed)
			{
				return;
			} 
			
			try
			{
				int size = client.socket.EndSend(result);

				client.RaiseMessageSent(size - Const.TotalSizeOfEnvelope);
			}
			catch (Exception e)
			{
				client.HandleError(e);
			}

			client.mreBeginSend.SetIfNotNull();
			client.mreEndSend.SetIfNotNull();
		}
	}
}
