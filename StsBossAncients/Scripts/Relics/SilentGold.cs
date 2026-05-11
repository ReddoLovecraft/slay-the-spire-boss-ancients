using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.ValueProps;
using StsBossAncients.Scripts.Main;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class SilentGold : StsBossAncientsRelic
{
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.ForEnergy(this), HoverTipFactory.FromKeyword(CardKeyword.Retain)];
	protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1)];
	public override RelicRarity Rarity => RelicRarity.Ancient;

	private bool _isActive;

	public override Task BeforeCombatStart()
	{
		_isActive = false;
		return Task.CompletedTask;
	}

	public override Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
	{
		if (side != CombatSide.Player)
		{
			return Task.CompletedTask;
		}
		CombatState? cs = Owner.Creature.CombatState;
		if (cs == null)
		{
			return Task.CompletedTask;
		}
		if (!CombatManager.Instance.IsPartOfPlayerTurn(Owner))
		{
			return Task.CompletedTask;
		}
		if (Owner.PlayerCombatState == null)
		{
			return Task.CompletedTask;
		}

		int played = CombatManager.Instance.History.CardPlaysFinished.Count((CardPlayFinishedEntry e) => e.HappenedThisTurn(cs) && e.CardPlay.Card.Owner == Owner);
		if (played != 0)
		{
			_isActive = false;
			return Task.CompletedTask;
		}

		_isActive = true;
		Flash();
		return Task.CompletedTask;
	}

	public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
	{
		if (player == Owner)
		{
			_isActive = false;
		}
		return Task.CompletedTask;
	}

	public override bool ShouldFlush(Player player)
	{
		if (player != Owner)
		{
			return true;
		}
		return !_isActive;
	}

	public override bool ShouldPlayerResetEnergy(Player player)
	{
		if (player != Owner)
		{
			return true;
		}
		CombatState? cs = player.Creature.CombatState;
		if (cs == null)
		{
			return true;
		}
		if (cs.RoundNumber == 1)
		{
			return true;
		}
		return !_isActive;
	}

	public override decimal ModifyHpLostAfterOsty(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
	{
		if (!_isActive)
		{
			return amount;
		}
		if (!CombatManager.Instance.IsInProgress)
		{
			return amount;
		}
		if (target != Owner.Creature)
		{
			return amount;
		}
		return 0m;
	}
}
