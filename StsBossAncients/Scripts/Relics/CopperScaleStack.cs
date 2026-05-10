using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using StsBossAncients.Scripts.Main;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class CopperScaleStack : StsBossAncientsRelic
	{
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<ThornsPower>()];
	protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<ThornsPower>(9m)];
	public override RelicRarity Rarity => RelicRarity.Ancient;

		public override async Task BeforeCombatStart()
		{
			Flash();
			await PowerCmd.Apply<ThornsPower>(Owner.Creature, 9, Owner.Creature, null);
		}
	}
}
