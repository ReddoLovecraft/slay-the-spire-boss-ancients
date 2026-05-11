using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Saves;
using StsBossAncients.Scripts.Main;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Rooms;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class GloryShield : StsBossAncientsRelic
	{
		protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StrengthPower>(), HoverTipFactory.Static(StaticHoverTip.Block)];
		public override RelicRarity Rarity => RelicRarity.Ancient;

		[SavedProperty]
		public int StrengthGained
		{
			get => _strengthGained;
			set
			{
				AssertMutable();
				_strengthGained = value;
			}
		}
		public override int DisplayAmount => StrengthGained;
		public override bool ShowCounter => false;
		private int _strengthGained;

		public override Task BeforeCombatStart()
		{
			StrengthGained = 0;
			InvokeDisplayAmountChanged();
			return Task.CompletedTask;
		}
		public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
		{
			if (target != Owner.Creature)
			{
				return;
			}
			if (dealer == null || dealer.Side == target.Side)
			{
				return;
			}
			if (!props.IsPoweredAttack())
			{
				return;
			}

			if (result.WasFullyBlocked)
			{
				Flash();
				StrengthGained += 4;
				InvokeDisplayAmountChanged();
				await PowerCmd.Apply<StrengthPower>(Owner.Creature, 4, Owner.Creature, null);
				return;
			}

			Flash();
			foreach (PowerModel p in Owner.Creature.Powers.ToList())
			{
				if (p.Type == PowerType.Debuff)
				{
					await PowerCmd.Remove(p);
				}
			}
			if (StrengthGained > 0)
			{
				PowerModel? str = Owner.Creature.GetPower<StrengthPower>();
				if (str != null)
				{
					await PowerCmd.ModifyAmount(str, -StrengthGained, null, null);
				}
				InvokeDisplayAmountChanged();
				StrengthGained = 0;
			}
		}
	}
}
