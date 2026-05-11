using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using System.Linq;
using System.Threading.Tasks;
using StsBossAncients.Scripts.Main;

namespace StsBossAncients.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class AwakeningTail : StsBossAncientsRelic
{
	public override RelicRarity Rarity => RelicRarity.Ancient;

	private bool _usedThisCombat;

	public override Task BeforeCombatStart()
	{
		_usedThisCombat = false;
		return Task.CompletedTask;
	}

	public override bool ShouldDieLate(Creature creature)
	{
		if (creature != Owner.Creature)
		{
			return true;
		}
		return _usedThisCombat;
	}

	public override async Task AfterPreventingDeath(Creature creature)
	{
		if (_usedThisCombat)
		{
			return;
		}
		_usedThisCombat = true;
		Flash();

		foreach (PowerModel p in creature.Powers.ToList())
		{
			if (p.Type == PowerType.Debuff)
			{
				await PowerCmd.Remove(p);
			}
		}
		await CreatureCmd.Heal(creature, creature.MaxHp);
	}
}
