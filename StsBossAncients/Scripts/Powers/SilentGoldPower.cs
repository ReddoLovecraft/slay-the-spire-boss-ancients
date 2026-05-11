using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Powers;

public sealed class SilentGoldPower : CustomPowerModel
{
	public override PowerType Type => PowerType.Buff;
	public override PowerStackType StackType => PowerStackType.Counter;

	public override decimal ModifyHpLostAfterOsty(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
	{
		if (target == Owner)
		{
			return 0m;
		}
		return amount;
	}

	public override async Task AfterEnergyReset(Player player)
	{
		if (player != Owner.Player)
		{
			return;
		}
		if (Amount > 0)
		{
			await PlayerCmd.GainEnergy(Amount, player);
		}
		await PowerCmd.Remove(this);
	}
}
