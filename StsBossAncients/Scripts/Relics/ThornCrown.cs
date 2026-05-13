using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using StsBossAncients.Scripts.Cards;
using StsBossAncients.Scripts.Main;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class ThornCrown : StsBossAncientsRelic
	{
		protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<Pain>(), HoverTipFactory.ForEnergy(this)];
		public override RelicRarity Rarity => RelicRarity.Ancient;
		public override bool HasUponPickupEffect => true;

		public override async Task AfterObtained()
		{
			await CardPileCmd.AddCursesToDeck(Enumerable.Repeat(ModelDb.Card<Pain>(),1), base.Owner);
		}
        protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(2)];
		public override async Task AfterRoomEntered(AbstractRoom room)
		{
			if (Owner.Creature.IsDead)
			{
				return;
			}
			Flash();
			await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Owner.Creature, 3, ValueProp.Unblockable | ValueProp.Unpowered, null, null);
		}

		public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
		{
			if (player != Owner)
			{
				return;
			}
			Flash();
			await CreatureCmd.Damage(choiceContext, Owner.Creature, 2, ValueProp.Unblockable | ValueProp.Unpowered, null, null);
			await PlayerCmd.GainEnergy(2, Owner);
		}

		public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
		{
			if (creature != Owner.Creature)
			{
				return;
			}
			if (delta >= 0)
			{
				return;
			}
			Flash();
			await CreatureCmd.GainMaxHp(Owner.Creature, 1);
		}
	}
}
