using System;
namespace AsyncClientServer
{
    interface IClient
    {
        event SocketEventHandler<Client, ClientIdEventArgs> ClientIdReceived;
        void Close();
        void Connect(string address, int port);
        event SocketEventHandler<Client, EventArgs> Connected;
        event SocketEventHandler<Client, EventArgs> Disconnected;
        void Dispose();
        int Id { get; }
        bool IsConnected();
        event SocketEventHandler<Client, ListClientIdEventArgs> ListClientIdReceived;
        event SocketEventHandler<Client, ReceivedEventArgs> MessageReceived;
        event SocketEventHandler<Client, EventArgs> MessageSent;
        void RequestListOfConnectedCliendId();
        void Send(byte[] message);
        void Send(string message);
        event SocketEventHandler<Client, SocketErrorEventArgs> SocketError;
    }
}
