using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves;
using StsBossAncients.Scripts.Main;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class HeroSword : StsBossAncientsRelic
	{
		protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StrengthPower>()];
		protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<StrengthPower>(0m), new PowerVar<StrengthPower>("HalfPower", 0m)];
		public override RelicRarity Rarity => RelicRarity.Ancient;

		[SavedProperty]
		public int Counter
		{
			get => _counter;
			set
			{
				AssertMutable();
				_counter = value;
				DynamicVars["Power"].BaseValue = value;
				DynamicVars["HalfPower"].BaseValue = value / 2;
				InvokeDisplayAmountChanged();
			}
		}

		private int _counter;

		public override bool ShowCounter => true;
		public override int DisplayAmount => Counter;

		public override async Task BeforeCombatStart()
		{
			CombatRoom? room = Owner.RunState.CurrentRoom as CombatRoom;
			if (room == null)
			{
				return;
			}

			int gain = 0;
			if (room.RoomType == RoomType.Elite || room.RoomType == RoomType.Boss)
			{
				gain = Counter;
			}
			else if (room.RoomType == RoomType.Monster)
			{
				gain = Counter / 2;
			}

			if (gain <= 0)
			{
				return;
			}

			Flash();
			await PowerCmd.Apply<StrengthPower>(Owner.Creature, gain, Owner.Creature, null);
		}

		public override Task AfterCombatVictory(CombatRoom room)
		{
			if (room.RoomType == RoomType.Elite)
			{
				Flash();
				Counter += 3;
			}
			else if (room.RoomType == RoomType.Boss)
			{
				Flash();
				Counter += 5;
			}
			return Task.CompletedTask;
		}
	}
}
