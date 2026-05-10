using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.RelicPools;
using StsBossAncients.Scripts.Main;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class FlameFlowerPot : StsBossAncientsRelic
	{
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<MegaCrit.Sts2.Core.Models.Powers.VitalSparkPower>()];
	public override RelicRarity Rarity => RelicRarity.Ancient;

		public override async Task BeforeCombatStart()
		{
		CombatState? cs = Owner.Creature.CombatState;
		if (cs == null)
		{
			return;
		}

			Flash();
			foreach (Creature enemy in cs.HittableEnemies.ToList())
			{
				await PowerCmd.Apply<MegaCrit.Sts2.Core.Models.Powers.VitalSparkPower>(enemy, 1, Owner.Creature, null);
			}
		}
	}
}
