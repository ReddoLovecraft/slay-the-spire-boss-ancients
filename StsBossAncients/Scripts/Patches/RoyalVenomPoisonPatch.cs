using HarmonyLib;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using StsBossAncients.Scripts.Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Patches
{
	[HarmonyPatch(typeof(PoisonPower), nameof(PoisonPower.AfterSideTurnStart))]
	public static class RoyalVenomPoisonPatch
	{
		public static bool Prefix(PoisonPower __instance, CombatSide side, CombatState combatState, ref Task __result)
		{
			__result = Run(__instance, side, combatState);
			return false;
		}

		private static async Task Run(PoisonPower poison, CombatSide side, CombatState combatState)
		{
			if (side != poison.Owner.Side)
			{
				return;
			}

			int accelerant = combatState.GetOpponentsOf(poison.Owner).Where(c => c.IsAlive).Sum(a => a.GetPowerAmount<AccelerantPower>());
			int triggerCount = Math.Min(poison.Amount, 1 + accelerant);
			bool royalVenomActive = combatState.Players.Any(p => p.Relics.Any(r => r is RoyalVenom));

			for (int i = 0; i < triggerCount; i++)
			{
				int poisonAmount = poison.Amount;
				decimal doomAmount = 0;
				if (royalVenomActive && poisonAmount > 0)
				{
					doomAmount = Hook.ModifyDamage(combatState.RunState, combatState, poison.Owner, null, poisonAmount, ValueProp.Unblockable | ValueProp.Unpowered, null, ModifyDamageHookType.All, MegaCrit.Sts2.Core.Entities.Cards.CardPreviewMode.None, out IEnumerable<AbstractModel> _);
				}

				await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), poison.Owner, poisonAmount, ValueProp.Unblockable | ValueProp.Unpowered, null, null);
				if (royalVenomActive && poison.Owner.IsAlive)
				{
					int stacks = Math.Max(0, (int)doomAmount);
					if (stacks > 0)
					{
						await PowerCmd.Apply<DoomPower>(poison.Owner, stacks, null, null);
					}
				}

				if (poison.Owner.IsAlive)
				{
					await PowerCmd.Decrement(poison);
				}
				else
				{
					await Cmd.CustomScaledWait(0.1f, 0.25f);
				}
			}
		}
	}
}

