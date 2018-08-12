using Lib_K_Relay.Networking.Packets.DataObjects;

namespace Lib_K_Relay.Networking.Packets.Server
{
    public class NewTickPacket : Packet
    {
        public int TickId;
        public int TickTime;
        public Status[] Statuses;

        public override PacketType Type
        { get { return PacketType.NEWTICK; } }

        public override void Read(PacketReader r)
        {
            TickId = r.ReadInt32();
            TickTime = r.ReadInt32();

            Statuses = new Status[r.ReadInt16()];
            for (int i = 0; i < Statuses.Length; i++)
                Statuses[i] = (Status)new Status().Read(r);
        }

        public override void Write(PacketWriter w)
        {
            w.Write(TickId);
            w.Write(TickTime);

            w.Write((short)Statuses.Length);
            foreach (Status s in Statuses) s.Write(w);
        }
    }
}
