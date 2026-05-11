using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using Patchoulib.Scrpits.Main;
using StsBossAncients.Scripts.Main;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class HellfireScepter : StsBossAncientsRelic
	{
		protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<VulnerablePower>(), HoverTipFactory.FromPower<WeakPower>(), HoverTipFactory.FromPower<FrailPower>()];
		public override RelicRarity Rarity => RelicRarity.Ancient;

		public override async Task BeforeCombatStart()
		{
			CombatState? cs = Owner.Creature.CombatState;
			if (cs == null)
			{
				return;
			}
			Flash();
			Tools.Talk("你是我的了！", Owner.Creature);
			foreach (Creature enemy in cs.HittableEnemies.ToList())
			{
				if (!enemy.IsAlive)
				{
					continue;
				}
				await PowerCmd.Apply<VulnerablePower>(enemy, 99, Owner.Creature, null);
				await PowerCmd.Apply<WeakPower>(enemy, 99, Owner.Creature, null);
				await PowerCmd.Apply<FrailPower>(enemy, 99, Owner.Creature, null);
			}
		}
	}
}
