using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncClientServer
{
	public delegate void SocketEventHandler<T, U>(T sender, U eventArgs) where U : EventArgs;

	public class ServerEventArgs : EventArgs
	{ 
		public Client Client {get; private set;}

		public static ServerEventArgs NewEvent(Client client)
		{
			return new ServerEventArgs() { Client = client };
		}		
	}
	public class ServerReceivedEventArgs : EventArgs
	{
		public Client Client { get; private set; }
		public byte[] Message { get; private set; }
		public MessageKind MessageKind { get; private set; }

		public static ServerReceivedEventArgs NewEvent(Client client, byte[] message, MessageKind messageKind)
		{
			return new ServerReceivedEventArgs() { Client = client, Message = message, MessageKind = messageKind };
		}
	}

	public class ServerSocketErrorEventArgs : EventArgs
	{
		public Client Client { get; private set; }
		public Exception Exception { get; private set; }

		public static ServerSocketErrorEventArgs NewEvent(Client client, Exception exception)
		{
			return new ServerSocketErrorEventArgs() { Client = client, Exception = exception };
		}
	}

	public class SocketErrorEventArgs : EventArgs
	{
		public Exception Exception { get; private set; }

		public static SocketErrorEventArgs NewEvent(Exception exception)
		{
			return new SocketErrorEventArgs() { Exception = exception };
		}
	}

	public class ClientIdEventArgs : EventArgs
	{
		public int Id { get; private set; }

		public static ClientIdEventArgs NewEvent(int id)
		{
			return new ClientIdEventArgs() { Id = id };
		}
	}

	public class ListClientIdEventArgs : EventArgs
	{
		public IEnumerable<int> Id { get; private set; }

		public static ListClientIdEventArgs NewEvent(IEnumerable<int> id)
		{
			return new ListClientIdEventArgs() { Id = id };
		}
	}

	public class ReceivedEventArgs : EventArgs
	{
		public byte[] Message { get; private set; }
		public MessageKind MessageKind { get; private set; }

		public static ReceivedEventArgs NewEvent(byte[] message, MessageKind messageKind)
		{
			return new ReceivedEventArgs() { Message = message , MessageKind = messageKind };
		}
	}
}
