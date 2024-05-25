using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Enums;
using WorldServer.Enums.Mob;
using WorldServer.Logic.WorldRuntime.ShopRuntime;

namespace WorldServer.Logic.WorldRuntime.MobDataRuntime
{
	internal class MobDataManager
	{
		private Dictionary<int, MobData> _mobData;

		public MobDataManager(WorldConfig _worldConfig)
		{
			_mobData = new Dictionary<int, MobData>();
			var tmobData = _worldConfig.GetConfig("[Mobs]");
			foreach (var entry in tmobData)
			{
				int Id = Convert.ToInt32(entry.Key);
				float MoveSpeed = Convert.ToSingle(entry.Value["MoveSpeed"]);
				float ChasSpeed = Convert.ToSingle(entry.Value["ChasSpeed"]);
				int Property = Convert.ToInt32(entry.Value["Property"]);
				if (!Enum.TryParse(entry.Value["AttkPattern"].Substring(1), out MobPattern AttkPattern))
					AttkPattern = MobPattern.PATERN_NULL;
				if (!Enum.TryParse(entry.Value["Aggressive"].Substring(1), out MobAggressive Aggressive))
					throw new Exception("undefined Aggressive");
				string Cooperate = new(entry.Value["Cooperate"]);
				string Escape = new(entry.Value["Escape"]);
				if (!Enum.TryParse(entry.Value["Attack"].Substring(1), out MobAttack Attack))
					throw new Exception("undefined Attack");
				int Scale = Convert.ToInt32(entry.Value["Scale"]);
				int FindCount = Convert.ToInt32(entry.Value["FindCount"]);
				int FindInterval = Convert.ToInt32(entry.Value["FindInterval"]);
				int MoveInterval = Convert.ToInt32(entry.Value["MoveInterval"]);
				int ChasInterval = Convert.ToInt32(entry.Value["ChasInterval"]);
				int AlertRange = Convert.ToInt32(entry.Value["AlertRange"]);
				int Limt0Range = Convert.ToInt32(entry.Value["Limt0Range"]);
				int Limt1Range = Convert.ToInt32(entry.Value["Limt1Range"]);
				int LEV = Convert.ToInt32(entry.Value["LEV"]);
				int EXP = Convert.ToInt32(entry.Value["EXP"]);
				int HP = Convert.ToInt32(entry.Value["HP"]);
				int Defense = Convert.ToInt32(entry.Value["Defense"]);
				int AttacksR = Convert.ToInt32(entry.Value["AttacksR"]);
				int DefenseR = Convert.ToInt32(entry.Value["DefenseR"]);
				int HPRechagR = Convert.ToInt32(entry.Value["HPRechagR"]);
				int Interval1 = Convert.ToInt32(entry.Value["Interval1"]);
				int PhyAttMin1 = Convert.ToInt32(entry.Value["PhyAttMin1"]);
				int PhyAttMax1 = Convert.ToInt32(entry.Value["PhyAttMax1"]);
				int Reach1 = Convert.ToInt32(entry.Value["Reach1"]);
				int Range1 = Convert.ToInt32(entry.Value["Range1"]);
				int Group1 = Convert.ToInt32(entry.Value["Group1"]);
				int Stance1 = Convert.ToInt32(entry.Value["Stance1"]);
				int Interval2 = Convert.ToInt32(entry.Value["Interval2"]);
				int PhyAttMin2 = Convert.ToInt32(entry.Value["PhyAttMin2"]);
				int PhyAttMax2 = Convert.ToInt32(entry.Value["PhyAttMax2"]);
				int Reach2 = Convert.ToInt32(entry.Value["Reach2"]);
				int Range2 = Convert.ToInt32(entry.Value["Range2"]);
				int Group2 = Convert.ToInt32(entry.Value["Group2"]);
				int Stance2 = Convert.ToInt32(entry.Value["Stance2"]);
				int Boss = Convert.ToInt32(entry.Value["Boss"]);
				int AtkSignal = Convert.ToInt32(entry.Value["AtkSignal"]);
				float Radius = Convert.ToSingle("0" + entry.Value["Radius"]);
				int Canatk = Convert.ToInt32(entry.Value["canatk"]);
				_mobData.Add(Id, new MobData(Id, MoveSpeed, ChasSpeed, Property, AttkPattern, Aggressive, Cooperate, Escape, Attack, Scale, FindCount, FindInterval,
					MoveInterval, ChasInterval, AlertRange, Limt0Range, Limt1Range, LEV, EXP, HP, Defense, AttacksR, DefenseR, HPRechagR, Interval1, PhyAttMin1, PhyAttMax1
					, Reach1, Range1, Group1, Stance1, Interval2, PhyAttMin2, PhyAttMax2, Reach2, Range2, Group2, Stance2, Boss, AtkSignal, Radius, Canatk));
			}
		}

		public MobData Get(int id)
		{
			return _mobData[id];
		}
	}
}
