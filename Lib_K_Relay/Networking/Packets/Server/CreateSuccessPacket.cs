namespace Lib_K_Relay.Networking.Packets.Server
{
    public class CreateSuccessPacket : Packet
    {
        public int ObjectId;
        public int CharId;

        public override PacketType Type
        { get { return PacketType.CREATESUCCESS; } }

        public override void Read(PacketReader r)
        {
            ObjectId = r.ReadInt32();
            CharId = r.ReadInt32();
        }

        public override void Write(PacketWriter w)
        {
            w.Write(ObjectId);
            w.Write(CharId);
        }
    }
}
