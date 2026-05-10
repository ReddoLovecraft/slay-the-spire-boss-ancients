using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using StsBossAncients.Scripts.Main;
using StsBossAncients.Scripts.Powers;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class AutomatedDefenseOverhaul : StsBossAncientsRelic
	{
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<FormShiftPower>(), HoverTipFactory.Static(StaticHoverTip.Block), HoverTipFactory.FromPower<ThornsPower>()];
	protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<FormShiftPower>(10m)];
	public override RelicRarity Rarity => RelicRarity.Ancient;

		public override async Task BeforeCombatStart()
		{
			Flash();
			await FormShiftPower.ApplyOrRefresh(Owner.Creature, 10, Owner.Creature, null);
		}
	}
}
