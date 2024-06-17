using LibPegasus.Enums;

namespace LibPegasus.Parsers.Mcl
{
	public class TileAttributeData
	{
		private UInt32[] _attrData = new uint[256 * 256];

		public TileAttributeData() { }

		public UInt32 GetTileAttribute(int x, int y)
		{
			return _attrData[x * 256 + y];
		}

		public bool HasTileAttribute(int x, int y, TileAttribute mask)
		{
			if ((GetTileAttribute(x, y) | (uint)mask) == GetTileAttribute(x, y))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool IsTileNormal(int x, int y)
		{
			return GetTileAttribute(x, y) == 0; //TEMP
		}

		public void SetTileAttribute(int index, UInt32 data)
		{
			_attrData[index] = data;
		}
	}
}
