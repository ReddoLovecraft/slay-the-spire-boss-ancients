using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
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
public sealed class DeadBell : StsBossAncientsRelic
{
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<DoomPower>()];
	public override RelicRarity Rarity => RelicRarity.Ancient;

	public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
	{
		if (dealer != Owner.Creature || !props.IsPoweredAttack_())
		{
			return;
		}
		if (result.TotalDamage <= 0)
		{
			return;
		}
		if (target.CurrentHp + result.UnblockedDamage >= target.MaxHp)
		{
			return;
		}
		Flash();
		await PowerCmd.Apply<DoomPower>(target, 12, Owner.Creature, cardSource);
	}
}
