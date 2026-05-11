using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.RelicPools;
using StsBossAncients.Scripts.Main;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class OldPhoto : StsBossAncientsRelic
{
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(CardKeyword.Exhaust)];
	public override RelicRarity Rarity => RelicRarity.Ancient;

	public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
	{
		if (player != Owner)
		{
			return;
		}
		if (Owner.Creature.CombatState == null)
		{
			return;
		}

		List<CardModel> options = PileType.Exhaust.GetPile(Owner).Cards.ToList();
		if (options.Count == 0)
		{
			return;
		}

		CardSelectorPrefs prefs = new CardSelectorPrefs(SelectionScreenPrompt, 0, 1)
		{
			RequireManualConfirmation = true,
			Cancelable = true
		};
		CardModel? selected = (await CardSelectCmd.FromSimpleGrid(choiceContext, options, Owner, prefs)).FirstOrDefault();
		if (selected == null)
		{
			return;
		}

		Flash();
		await CardPileCmd.Add(selected, PileType.Hand, CardPilePosition.Bottom, this);
		selected.EnergyCost.SetUntilPlayed(0);
	}
}
