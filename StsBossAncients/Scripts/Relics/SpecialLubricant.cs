using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.RelicPools;
using StsBossAncients.Scripts.Main;
using StsBossAncients.Scripts.Powers;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class SpecialLubricant : StsBossAncientsRelic
	{
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<LubricatedPower>()];
	public override RelicRarity Rarity => RelicRarity.Ancient;

		public override async Task BeforeCombatStart()
		{
			Flash();
			await LubricatedPower.ApplyOrRefresh(Owner.Creature, 9, Owner.Creature, null);
		}
	}
}
