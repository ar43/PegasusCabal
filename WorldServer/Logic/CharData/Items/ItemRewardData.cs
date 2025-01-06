using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Logic.CharData.Items
{
    internal class ItemRewardData
    {
        public ItemRewardData()
        {
            MainData = new();
        }

        public Dictionary<Tuple<uint, uint, uint>, ItemReward> MainData { get; private set; } // RewardIndex, Class, Order

        public void Add(Tuple<uint, uint, uint> id, ItemReward mainInfo)
        {
            if (MainData.ContainsKey(id)) throw new Exception("Key already contained");

            MainData.Add(id, mainInfo);
        }
    }

    internal class ItemReward
    {
        public ItemReward(UInt32 rewardItemIdx, UInt32 @class, string type, UInt32 grade, UInt32 order, UInt32 itemKind, UInt32 itemOpt, UInt32 duration)
        {
            RewardItemIdx = rewardItemIdx;
            Class = @class;
            Type = type;
            Grade = grade;
            Order = order;
            ItemKind = itemKind;
            ItemOpt = itemOpt;
            Duration = duration;
        }

        public uint RewardItemIdx { get; private set; }
        public uint Class { get; private set; }
        public string Type { get; private set; }
        public uint Grade { get; private set; }
        public uint Order { get; private set; }
        public uint ItemKind { get; private set; }
        public uint ItemOpt { get; private set; }
        public uint Duration { get; private set; }
    }
}
