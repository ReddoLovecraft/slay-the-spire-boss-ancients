using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.ValueProps;
using StsBossAncients.Scripts.Main;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class SandsOfTime : StsBossAncientsRelic
{
	public override RelicRarity Rarity => RelicRarity.Ancient;

	public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
	{
		if (cardPlay.Card.Owner != Owner)
		{
			return;
		}
		CombatState? cs = Owner.Creature.CombatState;
		if (cs == null)
		{
			return;
		}

		decimal pct = cs.RoundNumber / 100m;
		if (pct <= 0m)
		{
			return;
		}

		Flash();
		foreach (Creature e in cs.HittableEnemies)
		{
			if (e == null || !e.IsAlive)
			{
				continue;
			}
			decimal amount = e.MaxHp * pct;
			if (amount <= 0m)
			{
				continue;
			}
			await CreatureCmd.Damage(context, e, amount, ValueProp.Unblockable | ValueProp.Unpowered, Owner.Creature, cardPlay.Card);
		}
	}
}
