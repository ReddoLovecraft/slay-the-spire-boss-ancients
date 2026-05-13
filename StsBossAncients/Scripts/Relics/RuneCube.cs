using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.RelicPools;
using StsBossAncients.Scripts.Main;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class RuneCube : StsBossAncientsRelic
	{
		protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.ForEnergy(this)];
		public override RelicRarity Rarity => RelicRarity.Ancient;
		protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1)];

		public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
		{
			if (player != Owner)
			{
				return;
			}
			if (Owner.Creature.CurrentHp >= Owner.Creature.MaxHp)
			{
				return;
			}
			Flash();
			await PlayerCmd.GainEnergy(1, Owner);
		}
	}
}
