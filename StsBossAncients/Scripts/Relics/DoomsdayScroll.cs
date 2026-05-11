using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using StsBossAncients.Scripts.Main;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class DoomsdayScroll : StsBossAncientsRelic
	{
		protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<DoomPower>()];
		public override RelicRarity Rarity => RelicRarity.Ancient;

		public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
		{
			if (power is not DoomPower)
			{
				return;
			}
			if (amount <= 0)
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
			Flash();
			await CreatureCmd.LoseMaxHp(new ThrowingPlayerChoiceContext(), power.Owner, amount, isFromCard: false);
		}
	}
}
