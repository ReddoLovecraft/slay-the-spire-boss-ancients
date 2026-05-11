using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using StsBossAncients.Scripts.Main;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class KnightHeavyArmor : StsBossAncientsRelic
	{
		protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<PlatingPower>()];
		public override RelicRarity Rarity => RelicRarity.Ancient;

		public override decimal ModifyHandDraw(Player player, decimal count)
		{
			if (player != Owner)
			{
				return count;
			}
			CombatState? cs = Owner.Creature.CombatState;
			if (cs == null || cs.RoundNumber != 1)
			{
				return count;
			}
			return Math.Max(0, count - 2);
		}

		public override async Task BeforeCombatStart()
		{
			Flash();
			await PowerCmd.Apply<PlatingPower>(Owner.Creature, 19, Owner.Creature, null);
		}
	}
}
