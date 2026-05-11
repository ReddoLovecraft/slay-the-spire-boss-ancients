using BaseLib.Hooks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.ValueProps;
using StsBossAncients.Scripts.Main;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class HuntressBlade : StsBossAncientsRelic, IMaxHandSizeModifier
	{
		public override RelicRarity Rarity => RelicRarity.Ancient;

		public int ModifyMaxHandSize(Player player, int currentMaxHandSize)
		{
			if (player != Owner)
			{
				return currentMaxHandSize;
			}
			return currentMaxHandSize + 999;
		}

		public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
		{
			if (dealer != Owner.Creature)
			{
				return;
			}
			if (result.UnblockedDamage <= 0)
			{
				return;
			}
			Flash();
			await CardPileCmd.Draw(choiceContext, 1, Owner);
		}
	}
}
