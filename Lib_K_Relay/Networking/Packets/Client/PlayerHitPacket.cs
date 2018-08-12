namespace Lib_K_Relay.Networking.Packets.Client
{
    public class PlayerHitPacket : Packet
    {
        public byte BulletId;
        public int ObjectId;

        public override PacketType Type
        { get { return PacketType.PLAYERHIT; } }

        public override void Read(PacketReader r)
        {
            BulletId = r.ReadByte();
            ObjectId = r.ReadInt32();
        }

        public override void Write(PacketWriter w)
        {
            w.Write(BulletId);
            w.Write(ObjectId);
        }
    }
}
