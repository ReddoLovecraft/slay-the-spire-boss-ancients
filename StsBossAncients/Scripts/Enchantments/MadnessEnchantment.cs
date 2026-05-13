using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace StsBossAncients.Scripts.Enchantments
{
	public sealed class MadnessEnchantment : CustomEnchantmentModel
	{
		public override bool HasExtraCardText => true;
		protected override IEnumerable<IHoverTip> ExtraHoverTips => [];
		protected override string? CustomIconPath => "res://StsBossAncients/ArtWorks/Enchants/me.png";
		public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
		{
			modifiedCost = originalCost;
			if (card.Enchantment != this)
			{
				return false;
			}
			modifiedCost = 0;
			return true;
		}
	}
}

