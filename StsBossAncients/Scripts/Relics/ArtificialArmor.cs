using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.ValueProps;
using StsBossAncients.Scripts.Main;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class ArtificialArmor : StsBossAncientsRelic
	{
		protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<PlatingPower>()];
		public override RelicRarity Rarity => RelicRarity.Ancient;

		public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
		{
			if (amount <= 0)
			{
				return;
			}
			if (power.Owner != Owner.Creature)
			{
				return;
			}
			if (power.Type != PowerType.Debuff)
			{
				return;
			}
			Flash();
			await PowerCmd.Apply<PlatingPower>(Owner.Creature, 4, Owner.Creature, null);
		}

		public override async Task AfterCardChangedPiles(CardModel card, PileType oldPile, AbstractModel? source)
		{
			if (card.Owner != Owner)
			{
				return;
			}
			if (card.Pile?.Type != PileType.Hand)
			{
				return;
			}
			if (oldPile == PileType.Hand)
			{
				return;
			}
			if (card.Type != CardType.Status && card.Type != CardType.Curse)
			{
				return;
			}
			Flash();
			await CreatureCmd.GainBlock(Owner.Creature, 4, ValueProp.Unpowered, null);
		}
	}
}
