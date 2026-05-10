using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Patchouib.Scrpits.Main;
using BaseLib.Abstracts;

namespace StsBossAncients.Scripts.Enchantments
{
	public sealed class FireEnchantment : CustomEnchantmentModel
	{
	public override bool HasExtraCardText => true;
	protected override string? CustomIconPath => "res://StsBossAncients/ArtWorks/Enchants/fe.png";

	protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<IgnitePower>()];

		public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
		{
		if (dealer == null || cardSource == null)
		{
			return;
		}
		if (cardSource.Enchantment != this)
		{
			return;
		}
		if (result.UnblockedDamage <= 0)
		{
			return;
		}

			await PowerCmd.Apply<IgnitePower>(target, result.UnblockedDamage, dealer, cardSource);
		}
	}
}

