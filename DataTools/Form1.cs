using LibPegasus.JSON;
using System.Buffers.Binary;
using System.Text.Json;
using System.Windows.Forms;

namespace DataTools
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			comboBox1.Items.Add("InventoryData");
			comboBox1.Items.Add("EquipmentData");
			comboBox1.Items.Add("QuickSlotData");
			comboBox1.Items.Add("SkillData");
			comboBox1.Items.Add("Position");
		}

		private void button1_Click(object sender, EventArgs e)
		{
			var base64 = textBox1.Text;

			if (comboBox1.SelectedIndex <= -1)
			{
				MessageBox.Show("Select an option");
				return;
			}

			if (comboBox1.SelectedItem.ToString() == "InventoryData")
			{
				var byteArray = Convert.FromBase64String(base64);
				var entryLen = 18;
				if (byteArray.Length % entryLen != 0)
				{
					throw new Exception("byteArray.Length % 18 != 0");
				}

				var invData = new InventoryDataRoot(new Dictionary<uint, InventoryDataItem>());

				for (int i = 0; i < byteArray.Length; i += entryLen)
				{
					ReadOnlySpan<byte> span = new ReadOnlySpan<byte>(byteArray, i, entryLen);
					var kind = BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(0, 4));
					var unknown = BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(4, 4));
					var option = BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(8, 4));
					var slot = BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(12, 4));
					var unknown2 = BinaryPrimitives.ReadUInt16LittleEndian(span.Slice(16, 2));

					invData.InventoryData.Add(slot, new InventoryDataItem(kind, option));
				}

				string jsonString = JsonSerializer.Serialize(invData);
				//Clipboard.SetText(jsonString);
				if (jsonString.Length > 0)
				{
					textBox2.Text = jsonString.Substring(1, jsonString.Length - 2) + ",";
				}
			}
			else if (comboBox1.SelectedItem.ToString() == "EquipmentData")
			{
				var byteArray = Convert.FromBase64String(base64);
				var entryLen = 18;
				if (byteArray.Length % entryLen != 0)
				{
					throw new Exception("byteArray.Length % 18 != 0");
				}

				var eqData = new EquipmentDataRoot(new Dictionary<uint, EquipmentDataItem>());

				for (int i = 0; i < byteArray.Length; i += entryLen)
				{
					ReadOnlySpan<byte> span = new ReadOnlySpan<byte>(byteArray, i, entryLen);
					var kind = BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(0, 4));
					var unknown = BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(4, 4));
					var option = BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(8, 4));
					var slot = BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(12, 4));
					var unknown2 = BinaryPrimitives.ReadUInt16LittleEndian(span.Slice(16, 2));

					eqData.EquipmentData.Add(slot, new EquipmentDataItem(kind));
				}

				string jsonString = JsonSerializer.Serialize(eqData);
				//Clipboard.SetText(jsonString);
				if (jsonString.Length > 0)
				{
					textBox2.Text = jsonString.Substring(1, jsonString.Length - 2) + ",";
				}

			}
			else if (comboBox1.SelectedItem.ToString() == "QuickSlotData")
			{
				var byteArray = Convert.FromBase64String(base64);
				var entryLen = 4;
				if (byteArray.Length % entryLen != 0)
				{
					throw new Exception("byteArray.Length % 4 != 0");
				}

				var quickslotData = new QuickSlotDataRoot(new Dictionary<UInt16, QuickSlotDataEntry>());

				for (int i = 0; i < byteArray.Length; i += entryLen)
				{
					ReadOnlySpan<byte> span = new ReadOnlySpan<byte>(byteArray, i, entryLen);
					var id = BinaryPrimitives.ReadUInt16LittleEndian(span.Slice(0, 2));
					var slot = BinaryPrimitives.ReadUInt16LittleEndian(span.Slice(2, 2));

					quickslotData.QuickSlotData.Add(slot, new QuickSlotDataEntry(id));
				}

				string jsonString = JsonSerializer.Serialize(quickslotData);
				//Clipboard.SetText(jsonString);
				if (jsonString.Length > 0)
				{
					textBox2.Text = jsonString.Substring(1, jsonString.Length - 2) + ",";
				}

			}
			else if (comboBox1.SelectedItem.ToString() == "SkillData")
			{
				var byteArray = Convert.FromBase64String(base64);
				//speculation
				var entryLen = 5;
				if (byteArray.Length % entryLen != 0)
				{
					throw new Exception("byteArray.Length % 5 != 0");
				}

				var skillData = new SkillDataRoot(new Dictionary<UInt16, SkillDataEntry>());

				for (int i = 0; i < byteArray.Length; i += entryLen)
				{
					ReadOnlySpan<byte> span = new ReadOnlySpan<byte>(byteArray, i, entryLen);
					var id = BinaryPrimitives.ReadUInt16LittleEndian(span.Slice(0, 2));
					var level = span[2];
					var slot = BinaryPrimitives.ReadUInt16LittleEndian(span.Slice(3, 2));

					skillData.SkillData.Add(slot, new SkillDataEntry(id, level));
				}

				string jsonString = JsonSerializer.Serialize(skillData);
				//Clipboard.SetText(jsonString);
				if (jsonString.Length > 0)
				{
					textBox2.Text = jsonString.Substring(1, jsonString.Length - 2) + ",";
				}

			}
			else if (comboBox1.SelectedItem.ToString() == "Position")
			{
				var number = Convert.ToUInt32(base64);
				ushort y = (UInt16)(number & 0xFFFF);
				ushort x = (UInt16)((number & 0xFFFF0000) >> 16);

				var posData = new PositionDataRoot(new PositionDataEntry(x, y));

				string jsonString = JsonSerializer.Serialize(posData);
				//Clipboard.SetText(jsonString);
				if (jsonString.Length > 0)
				{
					textBox2.Text = jsonString.Substring(1, jsonString.Length - 2) + ",";
				}
			}
			else
			{
				MessageBox.Show("Select an option");
				return;
			}

		}

		private void textBox2_TextChanged(object sender, EventArgs e)
		{

		}

		private void button2_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog1 = new OpenFileDialog
			{
				InitialDirectory = @"D:\",
				Title = "Browse Text Files",

				CheckFileExists = true,
				CheckPathExists = true,

				DefaultExt = "json",
				Filter = "json files (*.json)|*.json",
				FilterIndex = 2,
				RestoreDirectory = true,

				ReadOnlyChecked = true,
				ShowReadOnly = true
			};

			if (openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				var jsonString = File.ReadAllText(openFileDialog1.FileName);
				var json = JsonSerializer.Deserialize<CharInitRoot>(jsonString);
				if(json != null)
				{
					//MessageBox.Show("JSON is ok");

					foreach (var item in json.CharInitData)
					{
						if(item.HP > UInt16.MaxValue)
						{
							item.HP = item.HP & 0xFFFF;
						}
						if (item.MP > UInt16.MaxValue)
						{
							item.MP = item.MP & 0xFFFF;
						}
					}
				}
				JsonSerializerOptions options = new()
				{
					WriteIndented = true
				};

				JsonSerializerOptions optionsCopy = new(options);
				File.WriteAllText(openFileDialog1.FileName, JsonSerializer.Serialize(json, optionsCopy));

				
			}
		}
	}
}
