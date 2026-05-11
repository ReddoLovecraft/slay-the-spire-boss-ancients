using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Enchantments;

public sealed class OneHeartEnchantment : CustomEnchantmentModel
{
	public override bool HasExtraCardText => true;

	private static int _triggerDepth;

	public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
	{
		if (cardPlay.Card.Enchantment != this)
		{
			return;
		}
		if (_triggerDepth > 0)
		{
			return;
		}
		if (cardPlay.PlayIndex != 0)
		{
			return;
		}

		Player? owner = cardPlay.Card.Owner;
		CombatState? cs = cardPlay.Card.CombatState ?? owner?.Creature.CombatState;
		if (owner == null || cs == null)
		{
			return;
		}

		_triggerDepth++;
		try
		{
			IEnumerable<CardModel> cards = owner.PlayerCombatState?.AllCards ?? new List<CardModel>();
			List<CardModel> targets = cards
				.Where(c => c != null
					&& c != cardPlay.Card
					&& c.Enchantment is OneHeartEnchantment
					&& c.Pile != null
					&& c.Pile.Type != PileType.Play)
				.ToList();

			foreach (CardModel c in targets)
			{
				await CardCmd.AutoPlay(new ThrowingPlayerChoiceContext(), c, null);
			}
		}
		finally
		{
			_triggerDepth--;
		}
	}
}
