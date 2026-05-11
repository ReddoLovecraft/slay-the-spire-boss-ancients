using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.RelicPools;
using StsBossAncients.Scripts.Main;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class Timer : StsBossAncientsRelic
{
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.ForEnergy(this)];
	public override RelicRarity Rarity => RelicRarity.Ancient;

	private CancellationTokenSource? _turnCts;

	public override Task AfterCombatEnd(MegaCrit.Sts2.Core.Rooms.CombatRoom room)
	{
		_turnCts?.Cancel();
		_turnCts?.Dispose();
		_turnCts = null;
		return Task.CompletedTask;
	}

	public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
	{
		if (player != Owner)
		{
			return Task.CompletedTask;
		}
		CombatState? cs = Owner.Creature.CombatState;
		if (cs == null)
		{
			return Task.CompletedTask;
		}

		_turnCts?.Cancel();
		_turnCts?.Dispose();
		_turnCts = new CancellationTokenSource();
		int round = cs.RoundNumber;
		_ = Countdown(round, _turnCts.Token);
		return Task.CompletedTask;
	}

	private async Task Countdown(int roundNumber, CancellationToken token)
	{
		await Cmd.Wait(30f, token);
		if (token.IsCancellationRequested)
		{
			return;
		}
		if (!CombatManager.Instance.IsInProgress)
		{
			return;
		}

		CombatState? cs = Owner.Creature.CombatState;
		if (cs == null || cs.RoundNumber != roundNumber)
		{
			return;
		}
		if (!CombatManager.Instance.IsPartOfPlayerTurn(Owner))
		{
			return;
		}

		Flash();
		PlayerCmd.EndTurn(Owner, canBackOut: false);
	}

	public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
	{
		modifiedCost = originalCost;
		if (card.Owner != Owner)
		{
			return false;
		}
		modifiedCost = 0m;
		return true;
	}
}
