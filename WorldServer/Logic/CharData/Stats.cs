using WorldServer.Enums;
using WorldServer.Logic.CharData.DbSyncData;
using WorldServer.Logic.CharData.Styles.Coefs;
using WorldServer.Logic.WorldRuntime;

namespace WorldServer.Logic.CharData
{
	internal class Stats
	{
		public Stats(Int32 level, UInt32 exp, int str, int dex, int @int, UInt32 pnt, UInt32 rank)
		{
			Level = level;
			Exp = exp;
			Str = str;
			Dex = dex;
			Int = @int;
			Pnt = (int)pnt;
			Rank = rank;
			Axp = 0;
			SyncPending = DBSyncPriority.NONE;
			LvlUpEventQueue = new();
		}

		public DBSyncPriority SyncPending { get; private set; }
		public Int32 Level { get; private set; }
		public UInt64 Exp { get; private set; }
		public UInt32 Axp { get; private set; }
		public int Str { get; private set; }
		public int Dex { get; private set; }
		public int Int { get; private set; }
		public int Pnt { get; private set; }
		public UInt32 Rank { get; private set; }
		private static UInt64[]? _expTable;
		public Queue<int> LvlUpEventQueue { get; private set; }

		public const int BASE_CR = 5;
		public const int BASE_CD = 120;
		public const int MAX_CRITICAL_RATE = 50;
		public void Sync(DBSyncPriority prio)
		{
			if (SyncPending < prio)
				SyncPending = prio;
			if (prio == DBSyncPriority.NONE)
				SyncPending = DBSyncPriority.NONE;
		}

		public void SetAbsoluteStats(int str, int dex, int intelligence, int pnt)
		{
			Dex = dex;
			Str = str;
			Int = intelligence;
			Pnt = pnt;
		}

		public bool SpendStatPoint(StatType stat, int value = 1)
		{
			if (value <= 0)
				return true;

			if(Pnt >= value)
			{
				switch (stat)
				{
					case StatType.STAT_STR:
					{
						Pnt -= value;
						Str += value;
						break;
					}
					case StatType.STAT_DEX:
					{
						Pnt -= value;
						Dex += value;
						break;
					}
					case StatType.STAT_INT:
					{
						Pnt -= value;
						Int += value;
						break;
					}
					default:
						return false;
				}
				return true;
			}
			else
			{
				return false;
			}
		}

		public static void LoadExpTable(WorldConfig worldConfig)
		{
			if (_expTable != null) throw new Exception("quest configs already loaded");
			_expTable = new ulong[250];

			var cfg = worldConfig.GetConfig("[ReqEXP]");
			_expTable[0] = 0;
			int i = 0;

			foreach (var it in cfg.Values)
			{
				if (i == 0)
				{
					i++;
					continue;
				}
				UInt64 ReqEXP = Convert.ToUInt64(it["ReqEXP"]);
				_expTable[i] = ReqEXP;
				i++;
			}
		}

		public DbSyncStats GetDB()
		{
			DbSyncStats stats = new DbSyncStats((Int32)Level, (Int64)Exp, (Int32)Axp, (Int32)Str, (Int32)Dex, (Int32)Int, (Int32)Pnt, (Int32)Rank);
			return stats;
		}

		public int CalculateValueFromCoef(StatCoef coef)
		{
			return ((coef.STR * Str) + (coef.DEX * Dex) + (coef.INT * Int)) / 10000;
		}

		private ulong GetNextLevelXPDiff()
		{
			ulong breakpointXp = 0;
			if(Level > 1)
			{
				for (int i = 1; i < Level; i++)
				{
					breakpointXp += _expTable[i];
				}

			}
			
			return (_expTable[Level] + breakpointXp) - Exp;
		}

		private void LevelUp()
		{
			Level++;
			Pnt += 5;
			LvlUpEventQueue.Enqueue(Level);
		}

		public void AddExp(ulong exp)
		{
			while (exp > 0)
			{
				var reqToLvl = GetNextLevelXPDiff();
				if (exp < reqToLvl)
				{
					Exp += exp;
					exp -= exp;
				}
				else
				{
					LevelUp();
					Exp += reqToLvl;
					exp -= reqToLvl;
				}
			}
		}
	}
}
