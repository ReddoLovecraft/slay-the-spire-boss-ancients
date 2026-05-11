using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using StsBossAncients.Scripts.Main;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class ChampionBelt : StsBossAncientsRelic
	{
		protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<VulnerablePower>(), HoverTipFactory.FromPower<WeakPower>()];
		public override RelicRarity Rarity => RelicRarity.Ancient;

		public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
		{
			if (amount <= 0)
			{
				return;
			}
			if (applier == null)
			{
				return;
			}
			if (applier != Owner.Creature)
			{
				return;
			}
			if (power.Owner.Side == applier.Side)
			{
				return;
			}

			if (power is VulnerablePower)
			{
				Flash();
				await PowerCmd.Apply<WeakPower>(power.Owner, 1, null, null);
			}
			else if (power is WeakPower)
			{
				Flash();
				await PowerCmd.Apply<VulnerablePower>(power.Owner, 1, null, null);
			}
		}
	}
}
