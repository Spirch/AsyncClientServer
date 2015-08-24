using System;
namespace AsyncClientServer
{
    interface IServer
    {
        void Close(int id);
        void CloseAll();
        event SocketEventHandler<Server, ServerEventArgs> Connected;
        event SocketEventHandler<Server, ServerEventArgs> Disconnected;
        void Dispose();
        event SocketEventHandler<Server, ServerReceivedEventArgs> MessageReceived;
        event SocketEventHandler<Server, ServerEventArgs> MessageSent;
        void Send(int id, string message);
        void SendAll(string message);
        event SocketEventHandler<Server, ServerSocketErrorEventArgs> SocketError;
        void Start(string address, int port);
        void Stop();
    }
}
