using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Logic.Char
{
	internal class Character
	{
		public Character(Style style, string name)
		{
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

	}
}
