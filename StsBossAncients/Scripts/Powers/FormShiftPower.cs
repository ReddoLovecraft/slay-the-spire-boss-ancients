using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System.Reflection;

namespace StsBossAncients.Scripts.Powers
{
	public sealed class FormShiftPower : CustomPowerModel
	{
	private static readonly MethodInfo? AmountSetter = typeof(PowerModel)
		.GetProperty("Amount", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		?.GetSetMethod(true);

	private int _cycleBase;
    public override string? CustomPackedIconPath => "res://StsBossAncients/ArtWorks/Powers/FSP32.png";
    public override string? CustomBigIconPath => "res://StsBossAncients/ArtWorks/Powers/FSP64.png";
	public override PowerType Type => PowerType.Buff;
	public override PowerStackType StackType => PowerStackType.Counter;
	public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;

	protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Block), HoverTipFactory.FromPower<ThornsPower>()];

	public static async Task<FormShiftPower> ApplyOrRefresh(Creature target, int amount, Creature? applier, CardModel? cardSource)
	{
		if (target.HasPower<FormShiftPower>())
		{
			FormShiftPower existing = target.GetPower<FormShiftPower>();
			existing.SetAmountUnsafe(existing.Amount + amount);
			return existing;
		}
		return await PowerCmd.Apply<FormShiftPower>(target, amount, applier, cardSource);
	}

	public override Task AfterApplied(Creature? applier, CardModel? cardSource)
	{
		_cycleBase = Amount;
		return Task.CompletedTask;
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
		if (amount <= 0m)
		{
			return;
		}

		int incoming = (int)Math.Floor(amount);
		if (incoming <= 0)
		{
			return;
		}

		if (incoming >= Amount)
		{
			await PowerCmd.Remove(this);
			return;
		}
		await PowerCmd.ModifyAmount(this, -incoming, null, null);
	}

	public override async Task AfterRemoved(Creature oldOwner)
	{
		if (!oldOwner.IsAlive)
		{
			return;
		}

		int cycleBase = _cycleBase;
		if (cycleBase <= 0)
		{
			return;
		}

		Flash();
		await CreatureCmd.GainBlock(oldOwner, cycleBase, ValueProp.Unpowered, null);
		if (cycleBase >= 2)
		{
			await PowerCmd.Apply<ThornsPower>(oldOwner, cycleBase / 2, oldOwner, null);
		}
		await ApplyOrRefresh(oldOwner, cycleBase + 10, oldOwner, null);
	}

		private void SetAmountUnsafe(int amount)
		{
			if (AmountSetter == null)
			{
				return;
			}
			AmountSetter.Invoke(this, [amount]);
			InvokeDisplayAmountChanged();
		}
	}
}

