using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using StsBossAncients.Scripts.Main;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class NonNewtonianFluid : StsBossAncientsRelic
	{
	public override RelicRarity Rarity => RelicRarity.Ancient;

	[SavedProperty]
	public int Counter
	{
		get => _counter;
		set
		{
			AssertMutable();
			_counter = value;
			InvokeDisplayAmountChanged();
		}
	}

	private int _counter;

	public override bool ShowCounter => true;
	public override int DisplayAmount => Counter;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Block)];
	public override async Task BeforeDamageReceived(PlayerChoiceContext choiceContext, Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
	{
		if (target != Owner.Creature)
		{
			return;
		}
		if (dealer == null || dealer.Side == Owner.Creature.Side)
		{
			return;
		}
		if (!props.IsPoweredAttack())
		{
			return;
		}

		if (Counter > 0)
		{
			Flash();
			await CreatureCmd.GainBlock(Owner.Creature, Counter, ValueProp.Unpowered, null);
		}

		int add = (int)Math.Floor(amount * 0.25m);
		Counter = Math.Max(0, Counter + add);
	}

		public override Task AfterCombatEnd(CombatRoom room)
		{
			Counter = 0;
			return Task.CompletedTask;
		}
	}
}
