using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System.Linq;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Enchantments;

public sealed class SacrificeEnchantment : CustomEnchantmentModel
{
	public override bool HasExtraCardText => true;

	public override decimal EnchantDamageAdditive(decimal originalDamage, ValueProp props)
	{
		if (!props.IsPoweredAttack())
		{
			return 0m;
		}
		return Amount * 5m;
	}
	protected override string? CustomIconPath => "res://StsBossAncients/ArtWorks/Enchants/se2.png";
	public override Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
	{
		if (cardSource?.Enchantment != this)
		{
			return Task.CompletedTask;
		}
		if (cardSource.Owner == null || dealer != cardSource.Owner.Creature)
		{
			return Task.CompletedTask;
		}
		if (!result.WasTargetKilled)
		{
			return Task.CompletedTask;
		}
		if (!target.Powers.All((PowerModel p) => p.ShouldOwnerDeathTriggerFatal()))
		{
			return Task.CompletedTask;
		}

		Amount++;
		if (cardSource.DeckVersion?.Enchantment != null)
		{
			cardSource.DeckVersion.Enchantment.Amount++;
		}
		return Task.CompletedTask;
	}
}
