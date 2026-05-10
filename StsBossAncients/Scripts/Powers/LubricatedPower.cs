using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace StsBossAncients.Scripts.Powers
{
	public sealed class LubricatedPower : CustomPowerModel
	{
	public override PowerType Type => PowerType.Buff;
	public override PowerStackType StackType => PowerStackType.Counter;
	public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
	 public override string? CustomPackedIconPath => "res://StsBossAncients/ArtWorks/Powers/LP32.png";
    public override string? CustomBigIconPath => "res://StsBossAncients/ArtWorks/Powers/LP64.png";

	public static async Task<LubricatedPower> ApplyOrRefresh(Creature target, int amount, Creature? applier, CardModel? cardSource)
	{
		return await PowerCmd.Apply<LubricatedPower>(target, amount, applier, cardSource);
	}

		public override async Task BeforeDamageReceived(PlayerChoiceContext choiceContext, Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
		{
		if (target != Owner)
		{
			return;
		}
		if (Amount <= 0)
		{
			return;
		}
		if (dealer == null || dealer.Side == Owner.Side)
		{
			return;
		}
		if (!props.IsPoweredAttack())
		{
			return;
		}

		int incoming = (int)Math.Floor(amount);
		if (incoming <= 1)
		{
			await PowerCmd.Decrement(this);
			return;
		}

		Flash();
		await CreatureCmd.GainBlock(Owner, incoming - 1, ValueProp.Unpowered, null);
			await PowerCmd.Decrement(this);
		}
	}
}
