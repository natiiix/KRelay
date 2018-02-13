namespace Lib_K_Relay.Networking.Packets.Client
{
    public class LeaveArenaPacket : Packet
    {
        public int Time;

        public override PacketType Type
        { get { return PacketType.LEAVEARENA; } }

        public override void Read(PacketReader r)
        {
            Time = r.ReadInt32();
        }

        public override void Write(PacketWriter w)
        {
            w.Write(Time);
        }
    }
}
