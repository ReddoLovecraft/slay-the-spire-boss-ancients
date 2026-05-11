using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using StsBossAncients.Scripts.Main;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class StarFruit : StsBossAncientsRelic
{
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.ForEnergy(this)];
	protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1), new StarsVar(1)];
	public override RelicRarity Rarity => RelicRarity.Ancient;

	public override async Task AfterStarsGained(int amount, Player gainer)
	{
		if (gainer != Owner)
		{
			return;
		}
		if (Owner.Creature.CombatState == null)
		{
			return;
		}
		if (amount <= 0)
		{
			return;
		}

		Flash();
		await PlayerCmd.GainEnergy(amount, Owner);
	}
}
