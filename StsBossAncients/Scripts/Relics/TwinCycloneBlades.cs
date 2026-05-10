using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.ValueProps;
using StsBossAncients.Scripts.Main;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class TwinCycloneBlades : StsBossAncientsRelic
	{
	public override RelicRarity Rarity => RelicRarity.Ancient;

	private CardModel? _trackedCard;
	private Creature? _trackedTarget;
	private int _trackedUnblockedDamage;

	public override Task BeforeCardPlayed(CardPlay cardPlay)
	{
		if (cardPlay.Card.Owner != Owner)
		{
			return Task.CompletedTask;
		}
		if (cardPlay.Target == null)
		{
			_trackedCard = null;
			_trackedTarget = null;
			_trackedUnblockedDamage = 0;
			return Task.CompletedTask;
		}

		_trackedCard = cardPlay.Card;
		_trackedTarget = cardPlay.Target;
		_trackedUnblockedDamage = 0;
		return Task.CompletedTask;
	}

	public override Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
	{
		if (_trackedCard == null || cardSource != _trackedCard)
		{
			return Task.CompletedTask;
		}
		if (dealer != Owner.Creature)
		{
			return Task.CompletedTask;
		}
		if (_trackedTarget == null || target != _trackedTarget)
		{
			return Task.CompletedTask;
		}

		_trackedUnblockedDamage += result.UnblockedDamage;
		return Task.CompletedTask;
	}

		public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
		{
			if (_trackedCard == null || cardPlay.Card != _trackedCard)
			{
				return;
			}

		int damage = _trackedUnblockedDamage;
		Creature? primaryTarget = _trackedTarget;
		_trackedCard = null;
		_trackedTarget = null;
		_trackedUnblockedDamage = 0;

		if (damage <= 0 || primaryTarget == null)
		{
			return;
		}

		CombatState? cs = Owner.Creature.CombatState;
		if (cs == null)
		{
			return;
		}

		List<Creature> others = cs.HittableEnemies.Where(e => e != primaryTarget).ToList();
		if (others.Count == 0)
		{
			return;
		}

			Flash();
			await CreatureCmd.Damage(context, others, damage, ValueProp.Unpowered, Owner.Creature, cardPlay.Card);
		}
	}
}
