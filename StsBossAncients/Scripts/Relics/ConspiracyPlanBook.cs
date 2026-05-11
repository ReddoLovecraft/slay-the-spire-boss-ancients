using BaseLib.Extensions;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using StsBossAncients.Scripts.Main;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.HoverTips;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class ConspiracyPlanBook : StsBossAncientsRelic
	{
		public override RelicRarity Rarity => RelicRarity.Ancient;
		protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
	{
		HoverTipFactory.FromKeyword(CardKeyword.Retain)
	});
		public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
		{
			if (side != CombatSide.Player)
			{
				return;
			}
			if (Owner.PlayerCombatState == null)
			{
				return;
			}
			if (Owner.PlayerCombatState.Hand.Cards.Count == 0)
			{
				return;
			}

			CardSelectorPrefs prefs = new CardSelectorPrefs(new LocString("relics", $"{Id.Entry}.selectionScreenPrompt"), 0, Owner.PlayerCombatState.Hand.Cards.Count)
			{
				RequireManualConfirmation = true,
				Cancelable = true
			};

			IEnumerable<CardModel> selected = await CardSelectCmd.FromHand(choiceContext, Owner, prefs, null, this);
			foreach (CardModel c in selected)
			{
				c.GiveSingleTurnRetain();
			}
		}
	}
}
