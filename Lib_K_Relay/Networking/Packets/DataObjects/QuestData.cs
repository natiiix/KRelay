using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib_K_Relay.Networking.Packets.DataObjects
{
    public class QuestData : IDataObject
    {
        public string Id;
        public string Name;
        public string Description;
        public int[] Requirements = new int[0];
        public int[] Rewards = new int[0];
        public bool Completed;
        public bool ItemOfChoice;
        public bool repeatable;
        public int Category;

        public IDataObject Read(PacketReader r)
        {
            Id = r.ReadString();
            Name = r.ReadString();
            Description = r.ReadString();
            Category = r.ReadInt32();
            Requirements = new int[r.ReadInt16()];
            for (int i = 0; i < Requirements.Length; i++)
                Requirements[i] = r.ReadInt32();
            Rewards = new int[r.ReadInt16()];
            for (int i = 0; i < Rewards.Length; i++)
                Rewards[i] = r.ReadInt32();
            Completed = r.ReadBoolean();
            ItemOfChoice = r.ReadBoolean();
            repeatable = r.ReadBoolean();

            return this;
        }

        public void Write(PacketWriter w)
        {
            w.Write(Id);
            w.Write(Name);
            w.Write(Description);
            w.Write(Category);
            w.Write((short)Requirements.Length);
            for (int i = 0; i < Requirements.Length; i++)
                w.Write(Requirements[i]);
            w.Write((short)Rewards.Length);
            for (int i = 0; i < Rewards.Length; i++)
                w.Write(Rewards[i]);
            w.Write(Completed);
            w.Write(ItemOfChoice);
            w.Write(repeatable);
        }

        public object Clone()
        {
            return new QuestData
            {
                Id = this.Id,
                Name = this.Name,
                Description = this.Description,
                Requirements = this.Requirements,
                Rewards = this.Rewards,
                Completed = this.Completed,
                ItemOfChoice = this.ItemOfChoice,
                Category = this.Category,
                repeatable = this.repeatable
            };
        }

        public override string ToString()
        {
            return "{ Id=" + Id + ", Name=" + Name + ", Description=" + Description + ", Requirements=" + Requirements.Select(x => x.ToString() + " ") + ", Rewards=" + Rewards.Select(x => x.ToString() + " ") + ", Completed=" + Completed.ToString() + ", ItemOfChoice=" + ItemOfChoice.ToString() + ", Category=" + Category.ToString() + ", Repeatable=" + repeatable.ToString() + " }";
        }
    }
}
