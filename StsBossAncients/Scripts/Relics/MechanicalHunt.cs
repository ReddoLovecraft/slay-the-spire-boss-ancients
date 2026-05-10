using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using StsBossAncients.Scripts.Main;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class MechanicalHunt : StsBossAncientsRelic
	{
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<ArtifactPower>()];
	public override RelicRarity Rarity => RelicRarity.Ancient;

		public override async Task BeforeCombatStart()
		{
			Flash();
			await PowerCmd.Apply<ArtifactPower>(Owner.Creature, 1, Owner.Creature, null);
		}
	}
}
