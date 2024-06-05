using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Logic.CharData.Styles.Coefs;

namespace WorldServer.Logic.CharData.Styles
{
	internal class BattleStyle
	{
		public BattleStyle(Int32 battleStyleId, StyleCoef attackCoef, StyleCoef magAttCoef, StyleCoef defensCoef, StyleCoef attckRCoef, StyleCoef defenRCoef, StatCoef statMaxAtt, StatCoef statMagAtt, StatCoef statDefens, StatCoef statAttckR, StatCoef statDefenR, Int32 initHP, Int32 initMP, Int32 deltaHP, Int32 deltaMP, Int32 minDmgRate)
		{
			Id = battleStyleId;
			AttackCoef = attackCoef;
			MagAttCoef = magAttCoef;
			DefensCoef = defensCoef;
			AttckRCoef = attckRCoef;
			DefenRCoef = defenRCoef;
			StatMaxAtt = statMaxAtt;
			StatMagAtt = statMagAtt;
			StatDefens = statDefens;
			StatAttckR = statAttckR;
			StatDefenR = statDefenR;
			InitHP = initHP;
			InitMP = initMP;
			DeltaHP = deltaHP;
			DeltaMP = deltaMP;
			MinDmgRate = minDmgRate;
		}

		public int Id { get; private set; }
		public StyleCoef AttackCoef { get; private set; }
		public StyleCoef MagAttCoef { get; private set; }

		public StyleCoef DefensCoef { get; private set; }
		public StyleCoef AttckRCoef { get; private set; }
		public StyleCoef DefenRCoef { get; private set; }
		public StatCoef StatMaxAtt { get; private set; } //Attack
		public StatCoef StatMagAtt { get; private set; } //MagicAttack
		public StatCoef StatDefens {  get; private set; }
		public StatCoef StatAttckR { get; private set; }
		public StatCoef StatDefenR { get; private set; }
		public int InitHP { get; private set; }
		public int InitMP { get; private set; }
		public int DeltaHP { get; private set; }
		public int DeltaMP { get; private set; }
		public int MinDmgRate { get; private set; }
	}
}
