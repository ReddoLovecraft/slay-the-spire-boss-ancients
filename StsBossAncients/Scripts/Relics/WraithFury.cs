using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
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
	public sealed class WraithFury : StsBossAncientsRelic
	{
	protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(0, ValueProp.Unblockable | ValueProp.Unpowered)];
	public override RelicRarity Rarity => RelicRarity.Ancient;

		public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature target, bool wasRemovalPrevented, float deathAnimLength)
		{
			if (!StsBossAncientsReflection.IsLikelyOsty(target, Owner))
			{
				return;
			}

		CombatState? cs = Owner.Creature.CombatState;
		if (cs == null)
		{
			return;
		}

			Flash();
			foreach (Creature enemy in cs.HittableEnemies.ToList())
			{
				int hpLoss = (int)Math.Floor(enemy.MaxHp / 6m);
				if (hpLoss <= 0)
				{
					continue;
				}
				await CreatureCmd.Damage(choiceContext, enemy, hpLoss, ValueProp.Unblockable | ValueProp.Unpowered, Owner.Creature, null);
			}
		}
	}
}
