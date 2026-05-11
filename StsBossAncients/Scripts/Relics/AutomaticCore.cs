using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Orbs;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using StsBossAncients.Scripts.Main;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class AutomaticCore : StsBossAncientsRelic
	{
		protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Channeling)];
		public override RelicRarity Rarity => RelicRarity.Ancient;

		public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
		{
			if (player != Owner)
			{
				return;
			}
			int slots = Owner.PlayerCombatState?.OrbQueue.Capacity ?? 0;
			if (slots <= 0)
			{
				return;
			}
			Flash();
			for (int i = 0; i < slots; i++)
			{
				OrbModel? orb = Owner.RunState.Rng.CombatTargets.NextItem(ModelDb.Orbs)?.ToMutable();
				if (orb != null)
				{
					await OrbCmd.Channel(choiceContext, orb, Owner);
				}
			}
		}
	}
}
