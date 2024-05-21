using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Enums
{
	public enum NpcType
	{
		NT_NONE = 0x00,                 // None
		NT_SHOP = 0x01,                 // Shop
		NT_SKIL = 0x02,                 // Skill
		NT_SMYU = 0x03,                 // StyleMasteyUp
		NT_ITUP = 0x04,                 // ItemUp ( + Shop )
		NT_WHOU = 0x05,                 // WareHouse
		NT_WARP = 0x06,                 // WarpNpc
		NT_ETCS = 0x07,                 // Etc
		NT_FTPN = 0x09,                 // ForceTower
		NT_FTSH = 0x0A,                 // ForceTower 
		NT_LCOR = 0x0B,                 //
		NT_WOBJ = 0x10,                 // WarpObject
		NT_QDUN = 0x11,                 // Quest Dungoen
	};
}
