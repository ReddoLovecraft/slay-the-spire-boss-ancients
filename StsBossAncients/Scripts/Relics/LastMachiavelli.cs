using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.RelicPools;
using StsBossAncients.Scripts.Main;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class LastMachiavelli : StsBossAncientsRelic
	{
	protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[3]
	{
		HoverTipFactory.FromCard<MinionStrike>(),
		HoverTipFactory.FromCard<MinionDiveBomb>(),
		HoverTipFactory.FromCard<MinionSacrifice>()
	};
	public override RelicRarity Rarity => RelicRarity.Ancient;

		public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
		{
			if (player != Owner)
			{
				return;
			}

		CombatState? cs = Owner.Creature.CombatState;
		if (cs == null)
		{
			return;
		}

		List<CardModel> options =
		[
			cs.CreateCard<MinionStrike>(Owner),
			cs.CreateCard<MinionDiveBomb>(Owner),
			cs.CreateCard<MinionSacrifice>(Owner)
		];

			CardSelectorPrefs prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1);
			List<CardModel> selected = (await CardSelectCmd.FromSimpleGrid(choiceContext, options, Owner, prefs)).ToList();
			if (selected.Count == 0)
			{
				return;
			}

			Flash();
			CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(selected[0], PileType.Hand, addedByPlayer: true));
		}
	}
}
