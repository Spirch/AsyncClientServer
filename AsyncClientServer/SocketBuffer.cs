using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncClientServer
{
    internal class SocketBuffer : IDisposable
    {
        internal readonly byte[] socketBuffer;
        internal readonly List<byte> outBuffer;
        internal MessageKind messageKind;
        internal int messageLength;	

        public SocketBuffer()
        {
            socketBuffer = new byte[Const.BufferSize];
            outBuffer = new List<byte>();
            messageKind = MessageKind.Unknown;
            messageLength = int.MaxValue;
        }

        public void Dispose()
        {
            outBuffer.Clear();
        }
    }
}
