using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using StsBossAncients.Scripts.Main;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class HeuristicAnalysis : StsBossAncientsRelic
	{
	public override RelicRarity Rarity => RelicRarity.Ancient;

		public override async Task AfterCardDrawnEarly(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
		{
			if (card.Owner != Owner)
			{
				return;
			}
			if (card.Type != CardType.Status && card.Type != CardType.Curse)
			{
				return;
			}

			Flash();
			await CardPileCmd.Draw(choiceContext, 2, Owner);
		}
	}
}
