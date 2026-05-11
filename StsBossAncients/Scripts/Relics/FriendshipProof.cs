using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.ValueProps;
using StsBossAncients.Scripts.Main;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class FriendshipProof : StsBossAncientsRelic
{
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<DieForYouPower>(), HoverTipFactory.Static(StaticHoverTip.Block)];
	public override RelicRarity Rarity => RelicRarity.Ancient;

	private async Task RemoveDieForYouIfNeeded()
	{
		Creature? osty = Owner.Osty;
		if (osty == null || !osty.IsAlive)
		{
			return;
		}
		await PowerCmd.Remove<DieForYouPower>(osty);
	}

	public override async Task AfterSummon(PlayerChoiceContext choiceContext, Player summoner, decimal amount)
	{
		if (summoner != Owner)
		{
			return;
		}
		await RemoveDieForYouIfNeeded();
	}

	public override async Task AfterOstyRevived(Creature osty)
	{
		if (osty.PetOwner != Owner)
		{
			return;
		}
		await RemoveDieForYouIfNeeded();
	}

	public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
	{
		if (side != CombatSide.Player)
		{
			return;
		}
		if (Owner.Creature.CombatState == null)
		{
			return;
		}

		Creature? osty = Owner.Osty;
		if (osty == null || !osty.IsAlive)
		{
			return;
		}

		Flash();
		await CreatureCmd.GainBlock(Owner.Creature, osty.MaxHp, ValueProp.Unpowered, null);
	}
}
