using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Orbs;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using StsBossAncients.Scripts.Main;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class LapisLazuli : StsBossAncientsRelic
{
	public override RelicRarity Rarity => RelicRarity.Ancient;

	protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
	{
		HoverTipFactory.FromPower<StrengthPower>(),
		HoverTipFactory.FromPower<DexterityPower>()
	};

	private int _drawCountThisCombat;

	public override Task BeforeCombatStart()
	{
		_drawCountThisCombat = 0;
		return Task.CompletedTask;
	}
	public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
	{
		if (card.Owner != Owner)
		{
			return;
		}
		if (Owner.Creature.CombatState == null)
		{
			return;
		}

		_drawCountThisCombat++;
		if (_drawCountThisCombat % 4 != 0)
		{
			return;
		}

		Flash();
		await PowerCmd.Apply<StrengthPower>(Owner.Creature, 1, Owner.Creature, null);
		await PowerCmd.Apply<DexterityPower>(Owner.Creature, 1, Owner.Creature, null);
	}
}
