using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using StsBossAncients.Scripts.Main;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class MechanicalDessert : StsBossAncientsRelic
{
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StrengthPower>(), HoverTipFactory.FromPower<PlatingPower>()];
	public override RelicRarity Rarity => RelicRarity.Ancient;

	public override async Task AfterObtained()
	{
		await CreatureCmd.GainMaxHp(Owner.Creature, 25);
		Flash();
	}

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

		int round = Owner.Creature.CombatState.RoundNumber;
		if (round % 2 == 1)
		{
			Flash();
			await PowerCmd.Apply<StrengthPower>(Owner.Creature, 3, Owner.Creature, null);
		}
		else
		{
			Flash();
			await PowerCmd.Apply<PlatingPower>(Owner.Creature, 3, Owner.Creature, null);
		}
	}
}
