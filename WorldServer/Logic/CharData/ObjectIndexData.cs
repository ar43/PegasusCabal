using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Enums;

namespace WorldServer.Logic.CharData
{
	internal class ObjectIndexData
	{
		public ObjectIndexData(UInt16 userId, Byte worldIndex, ObjectType objectType)
		{
			UserId = userId;
			WorldIndex = worldIndex;
			ObjectType = objectType;
		}

		public UInt16 UserId { get; private set; }
		public byte WorldIndex { get; private set; }
		public ObjectType ObjectType { get; private set; }

	}
}
