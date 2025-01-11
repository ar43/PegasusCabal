using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Enums;
using WorldServer.Logic.CharData.Items;
using WorldServer.Logic.SharedData;

namespace WorldServer.Logic.WorldRuntime.InstanceRuntime.GroundItemRuntime
{
	internal class GroundItemManager
	{
		Dictionary<int, GroundItem> _groundItems;
		private readonly Instance _instance;
		private BitArray _takenIds;
		private UInt16 _groundItemIdGenerator = 1;

		public GroundItemManager(Instance instance)
		{
			_instance = instance;
			_takenIds = new BitArray(0xFFFF + 1);
			_groundItems = new();
		}

		private UInt16 GetNextId()
		{
			_groundItemIdGenerator++;
			if (_takenIds[_groundItemIdGenerator] == false)
			{
				return _groundItemIdGenerator;
			}
			else
			{
				throw new NotImplementedException();
			}
				
			
		}


		public void AddGroundItem(Item item, UInt32 fromId, UInt16 X, UInt16 Y, ItemContextType itemContextType)
		{
			UInt16 newKey = (UInt16)_instance.Rng.Next(0xFFFF+1);
			ObjectIndexData oid = new(GetNextId(), (Byte)_instance.MapId, ObjectType.ITEM);

			GroundItem groundItem = new(oid, item, X, Y, itemContextType, newKey, fromId);
			_groundItems[oid.ObjectId] = groundItem;
			_instance.AddGroundItemToCell(groundItem, groundItem.CellX, groundItem.CellY, true);
		}
	}
}
