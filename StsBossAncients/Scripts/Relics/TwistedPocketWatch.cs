using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Saves;
using StsBossAncients.Scripts.Main;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class TwistedPocketWatch : StsBossAncientsRelic
{
	public override RelicRarity Rarity => RelicRarity.Ancient;
	public override bool ShowCounter => true;
	public override int DisplayAmount => CardsPlayed;

	[SavedProperty]
	public int CardsPlayed
	{
		get => _cardsPlayed;
		set
		{
			AssertMutable();
			_cardsPlayed = value;
			InvokeDisplayAmountChanged();
		}
	}

	[SavedProperty]
	public bool TakeExtraTurn
	{
		get => _takeExtraTurn;
		set
		{
			AssertMutable();
			_takeExtraTurn = value;
		}
	}

	private int _cardsPlayed;
	private bool _takeExtraTurn;

	public override Task BeforeCombatStart()
	{
		CardsPlayed = 0;
		TakeExtraTurn = false;
		return Task.CompletedTask;
	}

	public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
	{
		if (cardPlay.Card.Owner != Owner)
		{
			return Task.CompletedTask;
		}
		if (Owner.Creature.CombatState == null)
		{
			return Task.CompletedTask;
		}
		if (cardPlay.PlayIndex != 0)
		{
			return Task.CompletedTask;
		}

		CardsPlayed++;
		return Task.CompletedTask;
	}

	public override Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
	{
		if (side != CombatSide.Player)
		{
			return Task.CompletedTask;
		}
		if (Owner.Creature.CombatState == null)
		{
			return Task.CompletedTask;
		}
		if (!CombatManager.Instance.IsPartOfPlayerTurn(Owner))
		{
			return Task.CompletedTask;
		}

		if (CardsPlayed >= 12)
		{
			Flash();
			CardsPlayed -= 12;
			TakeExtraTurn = true;
		}
		return Task.CompletedTask;
	}

	public override bool ShouldTakeExtraTurn(Player player)
	{
		if (player != Owner)
		{
			return false;
		}
		if (!TakeExtraTurn)
		{
			return false;
		}
		TakeExtraTurn = false;
		return true;
	}
}
