using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncClientServer
{
	public static class ReceiveOperation
	{
		internal static void BeginReceive(this Client client)
		{
			client.outBuffer.Clear();
			client.KindOfMessage = KindMessage.Unknown;
			client.MessageLength = int.MaxValue;

			using (client.mreBeginReceive = new ManualResetEvent(false))
			{
				try
				{
					while (!client.closed)
					{
						client.mreBeginReceive.Reset();
						client.socket.BeginReceive(client.socketBuffer, 0, Const.BufferSize, SocketFlags.None, EndReceive, client);
						client.mreInit.SetIfNotNull();
						client.mreBeginReceive.WaitOne();
						client.mreIsConnected.WaitOne();
					}
				}
				catch (Exception e)
				{
					client.HandleError(e);
				}
			}
		}

		private static void EndReceive(IAsyncResult result)
		{
			var client = (Client)result.AsyncState;

			if (client.closed)
			{
				return;
			}

			try
			{
				var receive = client.socket.EndReceive(result);

				if (receive == 0)
				{
					client.Disconnect();
					return;
				}

				client.ProcessNewData(receive);
			}
			catch (Exception e)
			{
				client.HandleError(e);
			}

			client.mreBeginReceive.SetIfNotNull();
		}

		internal static void ProcessNewData(this Client client, int receive)
		{
			lock (client.outBuffer)
			{
				client.outBuffer.AddRange(client.socketBuffer.Take(receive));

				do
				{
					client.EnvelopeRead();

					if (client.outBuffer.Count >= client.MessageLength)
					{
						var msg = client.outBuffer.GetRange(0, client.MessageLength).ToArray();
						client.outBuffer.RemoveRange(0, client.MessageLength);

						client.RaiseMessageReceived(msg, client.KindOfMessage);

						client.KindOfMessage = KindMessage.Unknown;
						client.MessageLength = client.outBuffer.Count >= Const.TotalSizeOfEnvelope ? 0 :  int.MaxValue;
					}
				} while (client.outBuffer.Count >= client.MessageLength);
			}
		}

		private static void EnvelopeRead(this Client client)
		{
			if (client.KindOfMessage == KindMessage.Unknown && client.outBuffer.Count >= Const.TotalSizeOfEnvelope)
			{
				client.KindOfMessage = (KindMessage)client.outBuffer[0];

				if (!Enum.IsDefined(typeof(KindMessage), client.KindOfMessage))
				{
					client.KindOfMessage = KindMessage.Unknown;
					throw new FormatException("Doesn't understand the envelope!");
				}

				byte[] length = client.outBuffer.GetRange(Const.SizeOfEnvelopeKind, Const.SizeOfEnvelopeLength).ToArray();

				client.MessageLength = length.ToInt();

				if (client.MessageLength > ushort.MaxValue || client.MessageLength < 0)
				{
					throw new ArgumentOutOfRangeException();
				}

				client.outBuffer.RemoveRange(0, Const.TotalSizeOfEnvelope);
			}
		}
	}
}
