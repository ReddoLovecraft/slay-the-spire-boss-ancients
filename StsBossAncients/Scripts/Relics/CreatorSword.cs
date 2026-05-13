using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.RelicPools;
using StsBossAncients.Scripts.Main;
using StsBossAncients.Scripts.Utils;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class CreatorSword : StsBossAncientsRelic
{
	public override RelicRarity Rarity => RelicRarity.Ancient;
	protected override IEnumerable<IHoverTip> ExtraHoverTips => HoverTipFactory.FromForge();

	public override async Task AfterCardGeneratedForCombat(CardModel card, bool addedByPlayer)
	{
		if (card.Owner != Owner)
		{
			return;
		}
		if (!addedByPlayer)
		{
			return;
		}
		if (Owner.Creature.CombatState == null)
		{
			return;
		}

		Flash();
		foreach (var p in CombatUtils.GetAliveTeammatePlayers(Owner))
		{
			await ForgeCmd.Forge(10m, p, this);
		}
	}
}
