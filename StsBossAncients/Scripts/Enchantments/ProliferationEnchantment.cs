using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using BaseLib.Abstracts;

namespace StsBossAncients.Scripts.Enchantments;

public sealed class ProliferationEnchantment : CustomEnchantmentModel
{
	public override bool HasExtraCardText => true;
	protected override string? CustomIconPath => "res://StsBossAncients/ArtWorks/Enchants/pe.png";
	public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
	{
		if (cardPlay.Card.Enchantment != this)
		{
			return;
		}

		CombatState? combatState = cardPlay.Card.CombatState ?? cardPlay.Card.Owner?.Creature.CombatState;
		if (combatState == null)
		{
			return;
		}

		CardModel clone = combatState.CloneCard(cardPlay.Card);
		await CardPileCmd.AddGeneratedCardToCombat(clone, PileType.Draw, addedByPlayer: true, CardPilePosition.Random);
	}
}
