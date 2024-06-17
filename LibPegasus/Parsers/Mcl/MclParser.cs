using System.Text;

namespace LibPegasus.Parsers.Mcl
{
	public class MclParser
	{
		public TileAttributeData AttributeData { get; private set; }
		public MclParser()
		{
			AttributeData = new TileAttributeData();
		}

		public void Parse(string path)
		{
			if (File.Exists(path))
			{
				using (var stream = File.Open(path, FileMode.Open))
				{
					using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
					{
						_ = reader.ReadBytes(0x84);

						var effectCount = reader.ReadUInt32();
						for (int i = 0; i < effectCount; i++)
						{
							var length = reader.ReadUInt16();
							length += 18 - 2;
							_ = reader.ReadBytes((int)length);
						}

						var textureCount = reader.ReadUInt32();
						for (int i = 0; i < textureCount; i++)
						{
							var size = reader.ReadUInt32();
							_ = reader.ReadBytes((int)size);
						}

						_ = reader.ReadBytes(3 * 4);

						_ = reader.ReadBytes(257 * 257 * 4);

						for (int i = 0; i < 256 * 256; i++)
						{
							AttributeData.SetTileAttribute(i, Utils.Utility.ReverseBytes(reader.ReadUInt32()));
						}
					}
				}
			}
			else
			{
				throw new FileNotFoundException(path);
			}
		}

	}


}
