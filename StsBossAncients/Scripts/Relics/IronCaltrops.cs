using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.ValueProps;
using StsBossAncients.Scripts.Main;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class IronCaltrops : StsBossAncientsRelic
	{
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<DemisePower>()];
	protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<DemisePower>(4m)];
	public override RelicRarity Rarity => RelicRarity.Ancient;

		public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
		{
			if (target != Owner.Creature)
			{
				return;
			}
			if (dealer == null || dealer.Side == Owner.Creature.Side)
			{
				return;
			}
			if (!props.IsPoweredAttack())
			{
				return;
			}

			Flash();
			await PowerCmd.Apply<DemisePower>(dealer, 4, Owner.Creature, null);
		}
	}
}
