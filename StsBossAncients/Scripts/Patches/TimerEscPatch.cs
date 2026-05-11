using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.ValueProps;
using System.Linq;
using System.Threading.Tasks;
using TimerRelic = StsBossAncients.Scripts.Relics.Timer;

namespace StsBossAncients.Scripts.Patches;

[HarmonyPatch(typeof(NGame), nameof(NGame._Input))]
public static class TimerEscPatch
{
	public static void Postfix(InputEvent inputEvent)
	{
		if (!inputEvent.IsActionPressed(MegaInput.cancel))
		{
			return;
		}
		if (!MegaCrit.Sts2.Core.Combat.CombatManager.Instance.IsInProgress)
		{
			return;
		}

		CombatState? cs = CombatManager.Instance.DebugOnlyGetState();
		if (cs == null)
		{
			return;
		}
		Player? me = LocalContext.GetMe(cs.RunState);
		if (me == null)
		{
			return;
		}
		if (me.Relics.OfType<TimerRelic>().FirstOrDefault() == null)
		{
			return;
		}
		if (me.Creature.CombatState == null || me.Creature.CombatState.CurrentSide != CombatSide.Player)
		{
			return;
		}

		decimal dmg = me.Creature.MaxHp * 0.25m;
		if (dmg <= 0m)
		{
			return;
		}

		_ = DamageMe(me, dmg);
	}

	private static async Task DamageMe(Player me, decimal amount)
	{
		await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), me.Creature, amount, ValueProp.Unblockable | ValueProp.Unpowered, me.Creature, null);
	}
}
