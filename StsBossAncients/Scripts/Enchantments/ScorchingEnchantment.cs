using BaseLib.Abstracts;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace StsBossAncients.Scripts.Enchantments
{
	public sealed class ScorchingEnchantment : CustomEnchantmentModel
	{
		public override bool HasExtraCardText => true;

		public override bool CanEnchantCardType(CardType cardType)
		{
			return cardType == CardType.Attack;
		}
		protected override string? CustomIconPath => "res://StsBossAncients/ArtWorks/Enchants/se.png";
	}
    
	[HarmonyPatch(typeof(CardModel), "get_MaxUpgradeLevel")]
	public static class ScorchingEnchantment_CardModel_get_MaxUpgradeLevel_Patch
	{
		public static bool Prefix(CardModel __instance, ref int __result)
		{
			if (__instance.Enchantment is ScorchingEnchantment)
			{
				__result = 999;
				return false;
			}
			return true;
		}
	}
}
