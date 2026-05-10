using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using StsBossAncients.Scripts.Main;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class HellfireFirstStrike : StsBossAncientsRelic
	{
	protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(1, ValueProp.Unblockable | ValueProp.Unpowered)];
	public override RelicRarity Rarity => RelicRarity.Ancient;

	private CardModel? _trackedFirstAttack;
	private bool _triggeredThisCombat;

	public override Task BeforeCombatStart()
	{
		_trackedFirstAttack = null;
		_triggeredThisCombat = false;
		return Task.CompletedTask;
	}

	public override Task BeforeCardPlayed(CardPlay cardPlay)
	{
		if (_triggeredThisCombat)
		{
			return Task.CompletedTask;
		}
		if (_trackedFirstAttack != null)
		{
			return Task.CompletedTask;
		}
		if (cardPlay.Card.Owner != Owner)
		{
			return Task.CompletedTask;
		}
		if (cardPlay.Card.Type != CardType.Attack)
		{
			return Task.CompletedTask;
		}
		_trackedFirstAttack = cardPlay.Card;
		return Task.CompletedTask;
	}

	public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
	{
		if (_trackedFirstAttack == null || cardSource != _trackedFirstAttack)
		{
			return;
		}
		if (dealer != Owner.Creature)
		{
			return;
		}
		if (result.TotalDamage <= 0)
		{
			return;
		}

		_triggeredThisCombat = true;
		_trackedFirstAttack = null;
		int hpLoss = (int)Math.Floor(target.MaxHp / 12m) + 1;
		if (hpLoss <= 0)
		{
			return;
		}

		Flash();
		await CreatureCmd.Damage(choiceContext, target, hpLoss, ValueProp.Unblockable | ValueProp.Unpowered, Owner.Creature, cardSource);
	}

		public override Task AfterCombatEnd(CombatRoom room)
		{
			_trackedFirstAttack = null;
			_triggeredThisCombat = false;
			return Task.CompletedTask;
		}
	}
}
