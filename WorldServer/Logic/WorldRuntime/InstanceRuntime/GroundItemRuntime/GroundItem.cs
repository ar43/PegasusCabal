using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Enums;
using WorldServer.Logic.CharData.Items;
using WorldServer.Logic.SharedData;

namespace WorldServer.Logic.WorldRuntime.InstanceRuntime.GroundItemRuntime
{
	internal class GroundItem
	{
		public GroundItem(ObjectIndexData objectIndexData, Item item, UInt16 x, UInt16 y, ItemContextType itemContextType, UInt16 key, UInt32 fromId)
		{
			ObjectIndexData = objectIndexData;
			Item = item;
			X = x;
			Y = y;
			ItemContextType = itemContextType;
			Active = true;
			Key = key;
			FromId = fromId;
			CellX = (UInt16)(X / 16);
			CellY = (UInt16)(Y / 16);
		}

		public bool Active { get; private set; }
		public ObjectIndexData ObjectIndexData { get; private set; }
		public Item Item { get; private set; }
		public UInt16 X { get; private set; }
		public UInt16 Y { get; private set; }
		public UInt16 CellX { get; private set; }
		public UInt16 CellY { get; private set; }
		public UInt16 Key { get; private set; }
		public ItemContextType ItemContextType { get; private set; }
		public UInt32 FromId { get; private set; }

	}
}
