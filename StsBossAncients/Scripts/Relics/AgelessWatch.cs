using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using Patchouib.Scrpits.Main;
using StsBossAncients.Scripts.Main;
using System.Linq;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class AgelessWatch : StsBossAncientsRelic, IRightCilckable
{
	public override RelicRarity Rarity => RelicRarity.Ancient;

	private int _hpAtCombatStart;
	private bool _usedThisCombat;

	public override Task BeforeCombatStart()
	{
		_hpAtCombatStart = Owner.Creature.CurrentHp;
		_usedThisCombat = false;
		return Task.CompletedTask;
	}

	public async Task OnRightClick(PlayerChoiceContext context)
	{
		if (_usedThisCombat)
		{
			return;
		}
		if (Owner.Creature.CombatState?.RunState.CurrentRoom is not CombatRoom)
		{
			return;
		}

		_usedThisCombat = true;
		Flash();

		foreach (PowerModel p in Owner.Creature.Powers.ToList())
		{
			if (p.Type == PowerType.Debuff)
			{
				await PowerCmd.Remove(p);
			}
		}
		await CreatureCmd.SetCurrentHp(Owner.Creature, _hpAtCombatStart);
	}
}
