using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Enchantments;

public sealed class EchoEnchantment : CustomEnchantmentModel
{
	public override bool HasExtraCardText => true;

	private bool _activated;
	protected override string? CustomIconPath => "res://StsBossAncients/ArtWorks/Enchants/ee.png";
	public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
	{
		if (cardPlay.Card.Enchantment != this)
		{
			return Task.CompletedTask;
		}
		if (_activated)
		{
			return Task.CompletedTask;
		}
		if (cardPlay.IsAutoPlay)
		{
			return Task.CompletedTask;
		}
		if (cardPlay.PlayIndex != 0)
		{
			return Task.CompletedTask;
		}

		_activated = true;
		return Task.CompletedTask;
	}

	public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
	{
		if (!_activated)
		{
			return;
		}
		if (Card?.Owner == null || player != Card.Owner)
		{
			return;
		}
		if (Card.Pile == null || Card.Pile.Type == PileType.Play)
		{
			return;
		}
		if (player.Creature.CombatState == null)
		{
			return;
		}

		await CardCmd.AutoPlay(new ThrowingPlayerChoiceContext(), Card, null);
	}
}
