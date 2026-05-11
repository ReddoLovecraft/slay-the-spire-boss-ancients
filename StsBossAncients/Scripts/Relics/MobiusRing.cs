using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.RelicPools;
using StsBossAncients.Scripts.Main;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class MobiusRing : StsBossAncientsRelic
{
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.ReplayStatic)];
	public override RelicRarity Rarity => RelicRarity.Ancient;

	public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
	{
		if (cardPlay.Card.Owner != Owner)
		{
			return Task.CompletedTask;
		}
		if (Owner.Creature.CombatState == null)
		{
			return Task.CompletedTask;
		}
		if (cardPlay.PlayIndex != 0)
		{
			return Task.CompletedTask;
		}

		Flash();
		cardPlay.Card.BaseReplayCount += 1;
		return Task.CompletedTask;
	}
}
