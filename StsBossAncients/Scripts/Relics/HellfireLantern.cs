using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.RelicPools;
using Patchouib.Scrpits.Main;
using StsBossAncients.Scripts.Main;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class HellfireLantern : StsBossAncientsRelic
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

			int stacks = Math.Max(0, Owner.MaxEnergy * 2);
			if (stacks <= 0)
			{
				return;
			}

			Flash();
			foreach (Creature enemy in cs.HittableEnemies.ToList())
			{
				if (enemy.IsAlive)
				{
					await PowerCmd.Apply<IgnitePower>(enemy, stacks, Owner.Creature, null);
				}
			}
		}
	}
}
