using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Saves;
using StsBossAncients.Scripts.Main;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class ClockworkGear : StsBossAncientsRelic
	{
		public override RelicRarity Rarity => RelicRarity.Ancient;

		[SavedProperty]
		public int DrawCounter
		{
			get => _drawCounter;
			set
			{
				AssertMutable();
				_drawCounter = value;
			}
		}
		private int _drawCounter;

		public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
		{
			if (player == Owner)
			{
				DrawCounter = 0;
			}
			return Task.CompletedTask;
		}

		public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
		{
			if (card.Owner != Owner)
			{
				return;
			}
			DrawCounter++;
			if (DrawCounter % 3 == 0)
			{
				Flash();
				await CardPileCmd.Draw(choiceContext, 1, Owner);
			}
		}
	}
}
