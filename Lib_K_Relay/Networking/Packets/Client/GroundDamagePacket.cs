using Lib_K_Relay.Networking.Packets.DataObjects;

namespace Lib_K_Relay.Networking.Packets.Client
{
    public class GroundDamagePacket : Packet
    {
        public int Time;
        public Location Position;

        public override PacketType Type
        { get { return PacketType.GROUNDDAMAGE; } }

        public override void Read(PacketReader r)
        {
            Time = r.ReadInt32();
            Position = (Location)new Location().Read(r);
        }

        public override void Write(PacketWriter w)
        {
            w.Write(Time);
            Position.Write(w);
        }
    }
}
