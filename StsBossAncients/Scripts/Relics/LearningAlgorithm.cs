using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves;
using StsBossAncients.Scripts.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class LearningAlgorithm : StsBossAncientsRelic
	{
		public override RelicRarity Rarity => RelicRarity.Ancient;
		public override bool ShowCounter => true;
		public override int DisplayAmount => UpgradeCount;

		[SavedProperty]
		public int UpgradeCount
		{
			get => _upgradeCount;
			set
			{
				AssertMutable();
				_upgradeCount = value;
				InvokeDisplayAmountChanged();
			}
		}

		private int _upgradeCount = 1;

		public override Task AfterCombatEnd(CombatRoom room)
		{
			if (room.RoomType == RoomType.Monster || room.RoomType == RoomType.Elite || room.RoomType == RoomType.Boss)
			{
				UpgradeCount++;
			}
			return Task.CompletedTask;
		}

		public override Task AfterCombatVictory(CombatRoom room)
		{
			List<CardModel> candidates = Owner.Deck.Cards.Where(c => c.IsUpgradable).ToList();
			if (candidates.Count == 0)
			{
				return Task.CompletedTask;
			}
			Owner.RunState.Rng.CombatTargets.Shuffle(candidates);
			int n = Math.Min(UpgradeCount, candidates.Count);
			Flash();
			foreach (CardModel c in candidates.Take(n))
			{
				CardCmd.Upgrade(c);
			}
			return Task.CompletedTask;
		}
	}
}
