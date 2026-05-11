using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.ValueProps;
using StsBossAncients.Scripts.Main;
using StsBossAncients.Scripts.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class EightBodyDoll : StsBossAncientsRelic
{
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<PlatingPower>(), HoverTipFactory.Static(StaticHoverTip.Block)];
	public override RelicRarity Rarity => RelicRarity.Ancient;

	public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
	{
		if (player != Owner)
		{
			return;
		}
		if (Owner.Creature.CombatState == null)
		{
			return;
		}

		Flash();
		foreach (Player p in CombatUtils.GetAliveTeammatePlayers(Owner))
		{
			await PowerCmd.Apply<PlatingPower>(p.Creature, 3, Owner.Creature, null);
			await CreatureCmd.GainBlock(p.Creature, 4, ValueProp.Unpowered, null);
		}
	}
}
