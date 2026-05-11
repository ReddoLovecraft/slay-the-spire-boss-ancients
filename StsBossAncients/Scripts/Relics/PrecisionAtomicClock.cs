using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using StsBossAncients.Scripts.Main;
using System;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class PrecisionAtomicClock : StsBossAncientsRelic
	{
		protected override IEnumerable<DynamicVar> CanonicalVars => [new StarsVar(1)];
		public override RelicRarity Rarity => RelicRarity.Ancient;

		public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
		{
			if (player != Owner)
			{
				return;
			}
			CombatState? cs = Owner.Creature.CombatState;
			if (cs == null)
			{
				return;
			}
			int turns = Math.Max(0, cs.RoundNumber);
			if (turns <= 0)
			{
				return;
			}
			Flash();
			await PlayerCmd.GainStars(turns, Owner);
		}
	}
}
