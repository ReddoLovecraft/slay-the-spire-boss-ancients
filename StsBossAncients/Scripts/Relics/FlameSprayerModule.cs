using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.ValueProps;
using StsBossAncients.Scripts.Main;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class FlameSprayerModule : StsBossAncientsRelic
	{
	protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(10, ValueProp.Unpowered)];
	public override RelicRarity Rarity => RelicRarity.Ancient;

		public override async Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? source)
		{
		if (!CombatManager.Instance.IsInProgress)
		{
			return;
		}
		if (card.Owner != Owner)
		{
			return;
		}
		if (card.Pile?.Type != PileType.Hand)
		{
			return;
		}
		if (oldPileType == PileType.Hand)
		{
			return;
		}
		if (card.Type != CardType.Status && card.Type != CardType.Curse)
		{
			return;
		}

		CombatState? cs = Owner.Creature.CombatState;
		if (cs == null || cs.HittableEnemies.Count == 0)
		{
			return;
		}

			Flash();
			await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), cs.HittableEnemies.ToList(), 10, ValueProp.Unpowered, Owner.Creature, null);
		}
	}
}
