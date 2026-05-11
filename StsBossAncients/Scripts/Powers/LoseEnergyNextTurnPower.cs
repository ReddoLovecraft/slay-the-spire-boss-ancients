using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace StsBossAncients.Scripts.Powers
{
	public sealed class LoseEnergyNextTurnPower : CustomPowerModel
	{
		public override PowerType Type => PowerType.Debuff;
		public override PowerStackType StackType => PowerStackType.Counter;

		public override async Task AfterEnergyReset(Player player)
		{
			if (player != Owner.Player)
			{
				return;
			}
			if (Amount > 0)
			{
				await PlayerCmd.LoseEnergy(Amount, player);
			}
			await PowerCmd.Remove(this);
		}
	}
}

