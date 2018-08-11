using System;

namespace Lib_K_Relay.Networking.Packets.DataObjects
{
    public interface IDataObject : ICloneable
    {
        IDataObject Read(PacketReader r);

        void Write(PacketWriter w);
    }
}
