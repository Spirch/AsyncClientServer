using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace AsyncClientServer
{
	//supported message (kind of protocol)
	public enum MessageKind : byte
	{
		Unknown = 0,
		ServerReady = 10,
		ClientId = 20,
		ListClientId = 21,
		//AddClientId = 22, //not implemented yet
		//RemClientId = 23, //not implemented yet
		Message = 50,
	}
	public static class Const
	{
		public const int BufferSize = 1; //small buffer to test multi part message
		public const int SizeOfEnvelopeKind = sizeof(MessageKind);
		public const int SizeOfEnvelopeLength = sizeof(int);
		public const int TotalSizeOfEnvelope = SizeOfEnvelopeKind + SizeOfEnvelopeLength;
		public const int MonitorDisconnectCycle = 1000; //every X millisecond check if the connection is still open
		public const int BackLogLimit = 20; // backlog for the listener
	}

	internal static class MiscOperation
	{
		internal static void SetIfNotNull(this ManualResetEvent mre)
		{
			var localMre = mre;
			if (localMre != null && !localMre.SafeWaitHandle.IsClosed)
			{
				localMre.Set();
			}
		}

		internal static void HandleError(this Client client, Exception e)
		{
			if (!client.closed)
			{
				client.Close();
				client.OnSocketError(e);
			}
		}

		public static byte[] ToArrayOfByte(this IEnumerable<int> OriginalList)
		{
			int index = 0;
			var ArrayOfByte = new byte[OriginalList.Count() * 4];

			foreach (var item in OriginalList)
			{
				ArrayOfByte[index++] = (byte)item;
				ArrayOfByte[index++] = (byte)(item >> 8);
				ArrayOfByte[index++] = (byte)(item >> 16);
				ArrayOfByte[index++] = (byte)(item >> 24);
			}

			return ArrayOfByte;
		}

		public static IEnumerable<int> ToListOfInt(this byte[] ArrayOfByte)
		{
			int index = 0;
			int length = ArrayOfByte.Length;

			var CopyOfList = new List<int>(length / 4);

			while (index < length)
			{
				CopyOfList.Add(ArrayOfByte[index++] | (ArrayOfByte[index++] << 8) | (ArrayOfByte[index++] << 16) | (ArrayOfByte[index++] << 24));
			}

			return CopyOfList;
		}

		public static byte[] ToByte(this int value)
		{
			var ArrayOfByte = new byte[4];

			ArrayOfByte[0] = (byte)value;
			ArrayOfByte[1] = (byte)(value >> 8);
			ArrayOfByte[2] = (byte)(value >> 16);
			ArrayOfByte[3] = (byte)(value >> 24);

			return ArrayOfByte;
		}

		public static int ToInt(this byte[] value)
		{
			return value[0] | (value[1] << 8) | (value[2] << 16) | (value[3] << 24);
		}

		public static Socket NewSocket()
		{
			LingerOption lo = new LingerOption(true, 10);
			var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			socket.NoDelay = true;
			socket.LingerState = lo;
			socket.ReceiveBufferSize = Const.BufferSize;
			socket.SendBufferSize = Const.BufferSize;

			return socket;
		}
	}
}
