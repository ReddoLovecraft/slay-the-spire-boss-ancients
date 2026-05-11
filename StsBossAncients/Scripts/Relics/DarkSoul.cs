using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.RelicPools;
using StsBossAncients.Scripts.Main;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class DarkSoul : StsBossAncientsRelic
{
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.ForEnergy(this), HoverTipFactory.FromCard<Soul>(false)];
	protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1)];
	public override RelicRarity Rarity => RelicRarity.Ancient;

	public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
	{
		if (cardPlay.Card.Owner != Owner)
		{
			return;
		}
		if (Owner.Creature.CombatState == null)
		{
			return;
		}
		if (cardPlay.Card is not Soul)
		{
			return;
		}
		if (cardPlay.PlayIndex != 0)
		{
			return;
		}

		Flash();
		await PlayerCmd.GainEnergy(1, Owner);
	}
}
