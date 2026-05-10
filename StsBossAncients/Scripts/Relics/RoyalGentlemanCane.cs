using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using Patchouib.Scrpits.Main;
using StsBossAncients.Scripts.Main;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class RoyalGentlemanCane : StsBossAncientsRelic, IRightCilckable
	{
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [MegaCrit.Sts2.Core.MonsterMoves.Intents.StunIntent.GetStaticHoverTip()];
	public override RelicRarity Rarity => RelicRarity.Ancient;

	public override bool IsUsedUp => _isUsedUp;
	private bool _isUsedUp;

	public override Task AfterRoomEntered(AbstractRoom room)
	{
		if (room is CombatRoom)
		{
			_isUsedUp = false;
		}
		return Task.CompletedTask;
	}

		public async Task OnRightClick(PlayerChoiceContext context)
		{
		if (IsUsedUp)
		{
			return;
		}
		if (Owner.Creature.CombatState?.RunState.CurrentRoom is not CombatRoom)
		{
			return;
		}

		CombatState? cs = Owner.Creature.CombatState;
		if (cs == null)
		{
			return;
		}

			_isUsedUp = true;
			Flash();
			foreach (Creature enemy in cs.HittableEnemies.ToList())
			{
				if (enemy.IsAlive)
				{
					await CreatureCmd.Stun(enemy);
				}
			}
		}
	}
}
