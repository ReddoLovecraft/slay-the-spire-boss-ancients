using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.ValueProps;
using StsBossAncients.Scripts.Main;
using System.Linq;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class SelfMolt : StsBossAncientsRelic
{
	public override RelicRarity Rarity => RelicRarity.Ancient;

	public override Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
	{
		if (target != Owner.Creature)
		{
			return Task.CompletedTask;
		}
		if (result.UnblockedDamage <= 0)
		{
			return Task.CompletedTask;
		}

		var candidates = Owner.Deck.Cards.Where(c => c.IsUpgradable).ToList();
		if (candidates.Count == 0)
		{
			return Task.CompletedTask;
		}
		Owner.RunState.Rng.CombatTargets.Shuffle(candidates);
		Flash();
		CardCmd.Upgrade(candidates[0]);
		return Task.CompletedTask;
	}
}
