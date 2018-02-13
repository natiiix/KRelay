namespace Lib_K_Relay.Networking.Packets.Server
{
    public class TradeChangedPacket : Packet
    {
        public bool[] Offers;

        public override PacketType Type
        { get { return PacketType.TRADECHANGED; } }

        public override void Read(PacketReader r)
        {
            Offers = new bool[r.ReadInt16()];
            for (int i = 0; i < Offers.Length; i++)
                Offers[i] = r.ReadBoolean();
        }

        public override void Write(PacketWriter w)
        {
            w.Write((ushort)Offers.Length);
            foreach (bool i in Offers)
                w.Write(i);
        }
    }
}
