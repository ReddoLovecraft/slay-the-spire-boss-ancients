using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Models.Cards;
using System.Linq;

namespace StsBossAncients.Scripts.Patches
{
	[HarmonyPatch(typeof(BladeOfInk), "OnPlay")]
	public static class BladeOfInkPatches
	{
		public static bool Prefix(BladeOfInk __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref Task __result)
		{
			Player owner = __instance.Owner;
			if (owner.Relics.Any(r => r is StsBossAncients.Scripts.Relics.FlameDagger))
			{
				__result = CreateShivsWithoutInky(__instance);
				return false;
			}
			return true;
		}

		private static async Task CreateShivsWithoutInky(BladeOfInk card)
		{
			CombatState? combatState = card.CombatState ?? card.Owner.Creature.CombatState;
			if (combatState == null)
			{
				return;
			}
			await Shiv.CreateInHand(card.Owner, card.DynamicVars.Cards.IntValue, combatState);
		}
	}
}
