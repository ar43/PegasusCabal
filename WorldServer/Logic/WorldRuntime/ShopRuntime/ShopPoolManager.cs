namespace WorldServer.Logic.WorldRuntime.ShopRuntime
{
	internal class ShopPoolManager
	{
		Dictionary<int, ShopPool> _poolCollection;
		public ShopPoolManager(WorldConfig worldConfig)
		{
			_poolCollection = new Dictionary<int, ShopPool>();

			var poolData = worldConfig.GetConfig("[NPC]");
			foreach (var poolInfo in poolData)
			{
				var poolId = Convert.ToInt32(poolInfo.Value["Pool_ID1"]);
				var poolId2 = Convert.ToInt32(poolInfo.Value["Pool_ID2"]);
				var worldId = Convert.ToInt32(poolInfo.Value["World_ID"]);
				var npcId = Convert.ToInt32(poolInfo.Value["NPC_ID"]);
				CreatePool(new ShopPool(poolId, poolId2, worldId, npcId));
			}

			var items = worldConfig.GetConfig("[Shop]");
			foreach (var item in items)
			{
				var poolId = Convert.ToInt32(item.Value["Pool_ID"]);
				var pool = GetPool(poolId);

				var slotId = Convert.ToInt32(item.Value["SlotID"]);
				var itemKind = Convert.ToInt32(item.Value["ItemKind"]);
				var itemOpt = Convert.ToInt32(item.Value["ItemOpt"]);
				var durationIdx = Convert.ToInt32(item.Value["DurationIdx"]);
				var minLevel = Convert.ToInt32(item.Value["MinLevel"]);
				var maxLevel = Convert.ToInt32(item.Value["MaxLevel"]);
				var reputation = Convert.ToInt32(item.Value["Reputation"]);
				var onlyPremium = Convert.ToInt32(item.Value["OnlyPremium"]);
				var onlyWin = Convert.ToInt32(item.Value["OnlyWin"]);
				var alzPrice = Convert.ToInt32(item.Value["AlzPrice"]);
				var wExpPrice = Convert.ToInt32(item.Value["WExpPrice"]);
				var dPPrice = Convert.ToInt32(item.Value["DPPrice"]);
				var cashPrice = Convert.ToInt32(item.Value["CashPrice"]);
				var renew = Convert.ToInt32(item.Value["Renew"]);
				var characterBuyLimit = Convert.ToInt32(item.Value["ChracterBuyLimit"]);
				var sellLimit = Convert.ToInt32(item.Value["SellLimit"]);
				var marker = Convert.ToInt32(item.Value["Marker"]);
				var maxReputation = Convert.ToInt32(item.Value["MaxReputation"]);

				pool.AddItem(slotId, new(itemKind, itemOpt, durationIdx, minLevel, maxLevel, reputation, onlyPremium, onlyWin, alzPrice, wExpPrice, dPPrice, cashPrice, renew, characterBuyLimit, sellLimit, marker, maxReputation));
			}
		}

		public ShopPool GetPool(int poolId)
		{
			if (!_poolCollection.TryGetValue(poolId, out var pool))
			{
				throw new Exception("undefined pool");
			}
			return pool;
		}

		public void CreatePool(ShopPool newPool)
		{
			if (_poolCollection.ContainsKey(newPool.PoolId))
				throw new Exception("pool already exists");
			_poolCollection[newPool.PoolId] = newPool;
		}

		public int Count()
		{
			return _poolCollection.Count;
		}
	}
}
