using WorldServer.Enums;

namespace WorldServer.Logic.SharedData
{
	internal class ObjectIndexData
	{
		public ObjectIndexData(UInt16 objectId, Byte worldIndex, ObjectType objectType)
		{
			ObjectId = objectId;
			WorldIndex = worldIndex;
			ObjectType = objectType;
		}

		public UInt16 ObjectId { get; private set; }
		public byte WorldIndex { get; private set; }
		public ObjectType ObjectType { get; private set; }

	}
}
