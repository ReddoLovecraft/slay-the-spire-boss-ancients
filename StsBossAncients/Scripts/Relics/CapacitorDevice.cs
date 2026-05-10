using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using StsBossAncients.Scripts.Main;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class CapacitorDevice : StsBossAncientsRelic
	{
    	public override RelicRarity Rarity => RelicRarity.Ancient;
		protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(CardKeyword.Retain)];
		public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
		{
			if (side != Owner.Creature.Side)
			{
				return;
			}

			bool triggered = false;
			foreach (CardModel card in PileType.Hand.GetPile(Owner).Cards.ToList())
			{
				if (!card.ShouldRetainThisTurn)
				{
					continue;
				}

				triggered = true;
				if (card.IsUpgradable)
				{
					CardCmd.Upgrade(card);
				}
				card.EnergyCost.AddThisCombat(-1);
			}

			if (triggered)
			{
				Flash();
			}
		}
	}
}


