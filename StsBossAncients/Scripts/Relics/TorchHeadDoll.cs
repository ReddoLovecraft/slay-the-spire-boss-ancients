using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using Patchouib.Scrpits.Main;
using StsBossAncients.Scripts.Main;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class TorchHeadDoll : StsBossAncientsRelic
	{
		protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<IgnitePower>()];
		public override RelicRarity Rarity => RelicRarity.Ancient;

		public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
		{
			if (player != Owner)
			{
				return;
			}
			CombatState? cs = Owner.Creature.CombatState;
			if (cs == null)
			{
				return;
			}

			List<Creature> enemies = cs.HittableEnemies.Where(e => e.IsAlive).ToList();
			if (enemies.Count == 0)
			{
				return;
			}
			Owner.RunState.Rng.CombatTargets.Shuffle(enemies);
			foreach (Creature enemy in enemies.Take(2))
			{
				Flash();
				await PowerCmd.Apply<IgnitePower>(enemy, 7, Owner.Creature, null);
			}
		}
	}
}
