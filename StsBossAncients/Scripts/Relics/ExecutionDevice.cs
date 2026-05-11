using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using StsBossAncients.Scripts.Main;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class ExecutionDevice : StsBossAncientsRelic
	{
		protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<DoomPower>()];
		public override RelicRarity Rarity => RelicRarity.Ancient;

		public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
		{
			if (power is not DoomPower)
			{
				return;
			}
			await TryExecute(power.Owner);
		}

		public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
		{
			if (creature.Side == Owner.Creature.Side)
			{
				return;
			}
			await TryExecute(creature);
		}

		private async Task TryExecute(Creature creature)
		{
			if (!CombatManager.Instance.IsInProgress)
			{
				return;
			}
			if (!creature.IsAlive)
			{
				return;
			}
			int doom = creature.GetPowerAmount<DoomPower>();
			if (doom <= 0)
			{
				return;
			}
			if (doom > creature.CurrentHp)
			{
				Flash();
				await CreatureCmd.Kill(creature, force: true);
			}
		}
	}
}
