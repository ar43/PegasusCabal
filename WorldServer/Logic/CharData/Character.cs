using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Logic.CharData
{
	internal class Character
	{
		public Character(Style style, string name)
		{
			Style = style;
			Name = name;
			Location = new Location(0, 0, 0);
		}

		public Character(Style style, String name, Equipment? equipment, Inventory? inventory, Skills? skills, QuickSlotBar? quickSlotBar, Location location, CStats? stats, CStatus? status)
		{
			Equipment = equipment;
			Inventory = inventory;
			Skills = skills;
			QuickSlotBar = quickSlotBar;
			Location = location;
			Stats = stats;
			Status = status;
			Style = style;
			Name = name;
		}

		public bool Verify()
		{
			//TODO
			return Style.Verify();
		}

		public Style Style {  get; set; }
		public string Name { get; set; }
		public Equipment? Equipment { get; set; }
		public Inventory? Inventory { get; set; }
		public Skills? Skills { get; set; }
		public QuickSlotBar? QuickSlotBar { get; set; }
		public Location Location { get; private set; }
		public CStats? Stats { get; set; }
		public CStatus? Status { get; set; }

	}
}
