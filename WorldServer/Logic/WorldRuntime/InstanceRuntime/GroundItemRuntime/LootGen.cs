using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Logic.CharData.Items;
using WorldServer.Logic.Delegates;
using WorldServer.Logic.SharedData;
using WorldServer.Logic.WorldRuntime.InstanceRuntime.MobRuntime;
using WorldServer.Logic.WorldRuntime.LootDataRuntime;
using WorldServer.Logic.WorldRuntime.MapDataRuntime;

namespace WorldServer.Logic.WorldRuntime.InstanceRuntime.GroundItemRuntime
{
	internal static class LootGen
	{
		public static bool GenerateDropFromMob(List<LocalDropData> localDropTable, GroundItemManager groundItemManager, Mob mob, ref int roll)
		{
			foreach (var drop in localDropTable)
			{
				roll -= drop.DropRate;

				if (roll < 0)
				{
					if (drop.DurationIdx != 0)
						throw new NotImplementedException();

					var item = new Item((UInt32)drop.ItemKind, (UInt32)drop.ItemOpt, 0, 0);

					item.GenerateOption(mob.GetRNG(), drop.OptPoolIdx);

					groundItemManager.AddGroundItem(item, mob.ObjectIndexData.ObjectId, (UInt16)mob.Movement.X, (UInt16)mob.Movement.Y, Enums.ItemContextType.ItemFromMobs);
					return true;
				}
			}
			return false;
		}

		public static bool GenerateDropFromMob(List<MobDropData> mobDropData, GroundItemManager groundItemManager, Mob mob, ref int roll)
		{
			foreach (var drop in mobDropData)
			{
				roll -= drop.DropRate;

				if (roll < 0)
				{
					if (drop.DurationIdx != 0)
						throw new NotImplementedException();

					var item = new Item((UInt32)drop.ItemKind, (UInt32)drop.ItemOpt, 0, 0);

					item.GenerateOption(mob.GetRNG(), drop.OptPoolIdx);
					
					groundItemManager.AddGroundItem(item, mob.ObjectIndexData.ObjectId, (UInt16)mob.Movement.X, (UInt16)mob.Movement.Y, Enums.ItemContextType.ItemFromMobs);
					return true;
				}
			}
			return false;
		}

		public static bool GenerateDropFromMob(List<WorldDropData> worldDropData, GroundItemManager groundItemManager, Mob mob, ref int roll)
		{
			foreach (var drop in worldDropData)
			{
				roll -= drop.DropRate;

				if (roll < 0)
				{
					if (drop.DurationIdx != 0)
						throw new NotImplementedException();

					var item = new Item((UInt32)drop.ItemKind, (UInt32)drop.ItemOpt, 0, 0);

					item.GenerateOption(mob.GetRNG(), drop.OptPoolIdx);

					groundItemManager.AddGroundItem(item, mob.ObjectIndexData.ObjectId, (UInt16)mob.Movement.X, (UInt16)mob.Movement.Y, Enums.ItemContextType.ItemFromMobs);
					return true;
				}
			}
			return false;
		}
	}
}
