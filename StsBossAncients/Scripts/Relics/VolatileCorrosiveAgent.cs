using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using StsBossAncients.Scripts.Main;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class VolatileCorrosiveAgent : StsBossAncientsRelic
	{
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<PoisonPower>()];
	public override RelicRarity Rarity => RelicRarity.Ancient;

		public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
		{
		if (amount <= 0m)
		{
			return;
		}
		if (applier != Owner.Creature)
		{
			return;
		}
		if (cardSource == null || cardSource.Owner != Owner)
		{
			return;
		}
		if (power is not PoisonPower poison)
		{
			return;
		}
		if (poison.Owner.Side == Owner.Creature.Side)
		{
			return;
		}
		CombatState? cs = Owner.Creature.CombatState;
		if (cs == null)
		{
			return;
		}

		int stacks = (int)Math.Floor(amount);
		if (stacks <= 0)
		{
			return;
		}

		Flash();
		if (cardSource != null && cardSource.TargetType == TargetType.AllEnemies)
		{
			foreach (Creature enemy in cs.HittableEnemies.ToList())
			{
				await PowerCmd.Apply<PoisonPower>(enemy, stacks, Owner.Creature, null);
			}
			return;
		}

			foreach (Creature enemy in cs.HittableEnemies.Where(e => e != poison.Owner).ToList())
			{
				await PowerCmd.Apply<PoisonPower>(enemy, stacks, Owner.Creature, null);
			}
		}
	}
}
