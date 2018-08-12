namespace Lib_K_Relay.Networking.Packets.Server
{
    public class UpgradePetYaresultPacket : Packet
    {
        public int TypeId;

        public override PacketType Type
        { get { return PacketType.UPGRADEPETYARDRESULT; } }

        public override void Read(PacketReader r)
        {
            TypeId = r.ReadInt32();
        }

        public override void Write(PacketWriter w)
        {
            w.Write(TypeId);
        }
    }
}
