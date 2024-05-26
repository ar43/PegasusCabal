namespace WorldServer.Logic.WorldRuntime.ShopRuntime
{
	internal class ShopPool
	{
		public Dictionary<int, ShopEntry> Items { get; private set; }
		public int PoolId { get; private set; }
		public int Pool2Id { get; private set; }
		public int WorldId { get; private set; }
		public int NpcId { get; private set; }

		public ShopPool(Int32 poolId, Int32 pool2Id, Int32 worldId, Int32 npcId)
		{
			PoolId = poolId;
			Pool2Id = pool2Id;
			WorldId = worldId;
			NpcId = npcId;
			Items = new();
		}

		public void AddItem(int slot, ShopEntry item)
		{
			Items.Add(slot, item);
		}

		public ShopEntry GetItem(int slot)
		{
			return Items[slot];
		}

		public int Count()
		{
			return Items.Count;
		}
	}
}
