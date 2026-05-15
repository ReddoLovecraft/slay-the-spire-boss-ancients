using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.RelicPools;
using StsBossAncients.Scripts.Main;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class GluttonHeart : StsBossAncientsRelic
	{
	public override RelicRarity Rarity => RelicRarity.Ancient;

		public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature target, bool wasRemovalPrevented, float deathAnimLength)
		{
			if (target.Side == Owner.Creature.Side)
			{
				return;
			}

		int add = (int)Math.Floor(target.MaxHp * 0.1m);
		if (add <= 0)
		{
			return;
		}

			Flash();
			await CreatureCmd.GainMaxHp(Owner.Creature, add);
		}
	}
}
