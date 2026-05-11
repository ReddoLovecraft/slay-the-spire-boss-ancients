using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.RelicPools;
using StsBossAncients.Scripts.Main;
using StsBossAncients.Scripts.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class ShiningOintmentOctahedron : StsBossAncientsRelic
{
	public override RelicRarity Rarity => RelicRarity.Ancient;

	public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
	{
		if (player != Owner)
		{
			return;
		}

		var teammates = CombatUtils.GetAliveTeammatePlayers(Owner);
		if (teammates.Count == 0)
		{
			return;
		}

		List<CardModel> chosen = new List<CardModel>();
		foreach (Player p in teammates)
		{
			CardModel? picked = (await CardSelectCmd.FromDeckGeneric(
				player: p,
				prefs: new CardSelectorPrefs(SelectionScreenPrompt, 1, 1)
				{
					RequireManualConfirmation = true,
					Cancelable = false
				},
				filter: _ => true
			)).FirstOrDefault();
			if (picked != null)
			{
				chosen.Add(picked);
			}
		}

		if (chosen.Count == 0)
		{
			return;
		}

		Flash();
		foreach (CardModel original in chosen)
		{
			foreach (Player receiver in teammates)
			{
				var cs = receiver.Creature.CombatState;
				if (cs == null)
				{
					continue;
				}
				CardModel copy = cs.CloneCard(original);
				await CardPileCmd.AddGeneratedCardToCombat(copy, PileType.Hand, addedByPlayer: true);
			}
		}
	}
}
