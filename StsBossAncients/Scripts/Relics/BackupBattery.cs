using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves;
using Patchouib.Scrpits.Main;
using StsBossAncients.Scripts.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class BackupBattery : StsBossAncientsRelic, IRightCilckable
	{
		protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.ForEnergy(this)];
		public override RelicRarity Rarity => RelicRarity.Ancient;

		[SavedProperty]
		public int Penalty
		{
			get => _penalty;
			set
			{
				AssertMutable();
				_penalty = value;
			}
		}
		protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1)];
		[SavedProperty]
		public bool UsedThisTurn
		{
			get => _usedThisTurn;
			set
			{
				AssertMutable();
				_usedThisTurn = value;
			}
		}

		private int _penalty;
		private bool _usedThisTurn;

		public override Task AfterCombatEnd(CombatRoom room)
		{
			Penalty = 0;
			UsedThisTurn = false;
			return Task.CompletedTask;
		}

		public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
		{
			if (player == Owner)
			{
				UsedThisTurn = false;
			}
			return Task.CompletedTask;
		}

		public async Task OnRightClick(PlayerChoiceContext context)
		{
			if (UsedThisTurn)
			{
				return;
			}
			if (Owner.Creature.CombatState?.RunState.CurrentRoom is not CombatRoom)
			{
				return;
			}
			int max = Owner.PlayerCombatState?.MaxEnergy ?? Owner.MaxEnergy;
			int targetEnergy = Math.Max(0, max - Penalty);
			if (Owner.PlayerCombatState != null && Owner.PlayerCombatState.Energy < targetEnergy)
			{
				Flash();
				UsedThisTurn = true;
				await PlayerCmd.SetEnergy(targetEnergy, Owner);
				Penalty++;
			}
		}
	}
}
