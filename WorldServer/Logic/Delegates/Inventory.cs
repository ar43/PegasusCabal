using WorldServer.Enums;
using WorldServer.Packets.S2C;

namespace WorldServer.Logic.Delegates
{
	internal class Inventory
	{
		internal static void OnItemMove(Client client, Int32 fromType, Int32 fromSlot, Int32 toType, Int32 toSlot)
		{
			if (client.Character == null)
			{
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "null Character");
				return;
			}

			var inv = client.Character.Inventory;
			var eq = client.Character.Equipment;

			ItemMoveType fromMoveType;
			ItemMoveType toMoveType;
			try
			{
				fromMoveType = (ItemMoveType)fromType;
				toMoveType = (ItemMoveType)toType;
			}
			catch
			{
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "cant cast move type");
				return;
			}

			if (fromMoveType == ItemMoveType.INVENTORY && toMoveType == ItemMoveType.INVENTORY)
			{
				var success = client.Character.Inventory.ItemMove(fromSlot, toSlot, true);
				if (success)
				{
					var rsp = new RSP_ItemMove(1);
					client.PacketManager.Send(rsp);
				}
				else
				{
					client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "failed inv move");
					return;
				}
			}
			else if (fromMoveType == ItemMoveType.INVENTORY && toMoveType == ItemMoveType.EQUIPMENT)
			{
				var item = inv.RemoveItem((ushort)fromSlot);
				if (item == null)
				{
					client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "failed to equip item - could not remove item from inv");
					return;
				}

				bool success = eq.EquipItem(client.Character, item, (ushort)toSlot);
				if (success)
				{
					var nfy = new NFY_ItemEquips0(client.Character.Id, item.Kind, (ushort)toSlot);
					var rsp = new RSP_ItemMove(1);
					client.BroadcastNearby(nfy);
					client.PacketManager.Send(rsp);
				}
				else
				{
					inv.AddItem((ushort)fromSlot, item); //add the item back in case of failure
					client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "failed item equip");
					return;
				}
			}
			else if (fromMoveType == ItemMoveType.EQUIPMENT && toMoveType == ItemMoveType.INVENTORY)
			{
				var item = eq.UnequipItem((ushort)fromSlot);
				if (item == null)
				{
					client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "failed to unequip item - could not remove item from eq");
					return;
				}

				bool success = inv.AddItem((ushort)toSlot, item);
				if (success)
				{
					var nfy = new NFY_ItemUnequip(client.Character.Id, (ushort)toSlot);
					var rsp = new RSP_ItemMove(1);
					client.BroadcastNearby(nfy);
					client.PacketManager.Send(rsp);
				}
				else
				{
					eq.EquipItem(client.Character, item, (ushort)fromSlot);
					client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "failed to unequip item");
					return;
				}

			}
			else
			{
				throw new NotImplementedException();
			}

		}

		internal static void OnItemSwap(Client client, Int32 fromType1, Int32 fromSlot1, Int32 toType1, Int32 toSlot1, Int32 fromType2, Int32 fromSlot2, Int32 toType2, Int32 toSlot2)
		{
			if (client.Character == null)
			{
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "null Character");
				return;
			}

			ItemMoveType fromMoveType1;
			ItemMoveType toMoveType1;
			ItemMoveType fromMoveType2;
			ItemMoveType toMoveType2;
			try
			{
				fromMoveType1 = (ItemMoveType)fromType1;
				toMoveType1 = (ItemMoveType)toType1;
				fromMoveType2 = (ItemMoveType)fromType2;
				toMoveType2 = (ItemMoveType)toType2;
			}
			catch
			{
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "cant cast move type");
				return;
			}

			if (fromMoveType1 == ItemMoveType.INVENTORY && toMoveType1 == ItemMoveType.INVENTORY && fromMoveType2 == ItemMoveType.INVENTORY && toMoveType2 == ItemMoveType.INVENTORY)
			{
				var success = client.Character.Inventory.ItemSwap(fromSlot1, toSlot1, fromSlot2, toSlot2, true);
				if (success)
				{
					var rsp = new RSP_ItemSwap(1);
					client.PacketManager.Send(rsp);
				}
				else
				{
					client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "failed inv move");
					return;
				}
			}
			else
			{
				throw new NotImplementedException();
			}
		}
	}
}
