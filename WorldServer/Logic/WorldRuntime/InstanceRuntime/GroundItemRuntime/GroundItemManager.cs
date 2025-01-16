using System.Collections;
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
			UInt16 newKey = (UInt16)_instance.Rng.Next(0xFFFF + 1);
			ObjectIndexData oid = new(GetNextId(), (Byte)_instance.MapId, ObjectType.ITEM);

			GroundItem groundItem = new(oid, item, X, Y, itemContextType, newKey, fromId);
			_groundItems[oid.ObjectId] = groundItem;
			_instance.AddGroundItemToCell(groundItem, groundItem.CellX, groundItem.CellY, true);
		}

		public void RemoveGroundItem(GroundItem groundItem)
		{
			groundItem.Delete();
			_instance.RemoveGroundItemFromCell(groundItem, true);
			_groundItems.Remove(groundItem.ObjectIndexData.ObjectId);
		}

		internal Item? OnLootRequest(Client client, ObjectIndexData objectIndexData, UInt16 key, UInt32 itemKind, UInt16 slot)
		{
			var groundItem = _groundItems[objectIndexData.ObjectId];
			var questLootInfo = (0, 0, 0);

			if (groundItem == null)
				throw new Exception("ground item not found");

			if (groundItem.Key != key)
				throw new Exception("incorrect key");

			if (groundItem.Item.Kind != itemKind)
				throw new Exception("item kind mismatch");

			if (groundItem.Active == false)
				throw new Exception("item already looted");

			if (groundItem.Item.IsQuestItem())
			{
				questLootInfo = client.Character.QuestManager.NeedItem(groundItem.Item);
				if (questLootInfo == (0, 0, 0))
					throw new Exception("don't need this quest item");
			}

			if (client.Character?.Location?.Instance?.Id == _instance.Id)
			{
				Item item = groundItem.Item;
				bool addSuccess = client.Character.Inventory.AddItem(slot, item);
				if (!addSuccess)
					throw new Exception("inventory desync (slot not empty?)");

				RemoveGroundItem(groundItem);

				if (questLootInfo != (0, 0, 0))
				{
					client.Character.QuestManager.OnQuestItemLoot(questLootInfo.Item1, questLootInfo.Item2, questLootInfo.Item3);
				}

				return item;
			}
			else
			{
				throw new Exception("instance mismatch");
			}
		}
	}
}
