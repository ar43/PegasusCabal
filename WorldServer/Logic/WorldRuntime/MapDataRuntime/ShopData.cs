namespace WorldServer.Logic.WorldRuntime.MapDataRuntime
{
	internal class ShopData
	{
		public ShopData(Int32 poolId1, Int32 poolId2)
		{
			PoolId1 = poolId1;
			PoolId2 = poolId2;
		}

		public int PoolId1 { get; private set; }
		public int PoolId2 { get; private set; }
	}
}
