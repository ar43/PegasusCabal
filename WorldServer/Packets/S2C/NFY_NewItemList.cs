using LibPegasus.Packets;
using Nito.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Enums;
using WorldServer.Logic.WorldRuntime.InstanceRuntime.GroundItemRuntime;

namespace WorldServer.Packets.S2C
{
	internal class NFY_NewItemList : PacketS2C
	{
		List<GroundItem> _items;
		uint _fromIdOverride;

		public NFY_NewItemList(List<GroundItem> items, uint fromIdOverride = 0) : base((UInt16)Opcode.NFY_NEWITEMLIST)
		{
			_items = items;
			_fromIdOverride = fromIdOverride;
		}

		public override void WritePayload(Deque<byte> data)
		{
			int i = 0;
			PacketWriter.WriteByte(data, (byte)_items.Count);
			foreach(var item in _items)
			{
				if (i == 256)
					throw new Exception("hit local ent limit, FIXME");
				PacketWriter.WriteUInt16(data, item.ObjectIndexData.ObjectId);
				PacketWriter.WriteByte(data, item.ObjectIndexData.WorldIndex);
				PacketWriter.WriteByte(data, (Byte)item.ObjectIndexData.ObjectType);
				PacketWriter.WriteUInt32(data, item.Item.Option);
				if(_fromIdOverride == 0)
					PacketWriter.WriteUInt32(data, item.FromId);
				else
					PacketWriter.WriteUInt32(data, _fromIdOverride);
				PacketWriter.WriteUInt32(data, item.Item.Kind);
				PacketWriter.WriteUInt16(data, item.X);
				PacketWriter.WriteUInt16(data, item.Y);
				PacketWriter.WriteUInt16(data, item.Key);
				PacketWriter.WriteByte(data, (byte)item.ItemContextType);
				PacketWriter.WriteByte(data, 6); //TODO: what is this
				i++;
			}
		}
	}
}
