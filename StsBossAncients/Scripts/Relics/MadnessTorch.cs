using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Saves.Runs;
using Patchouib.Scrpits.Main;
using StsBossAncients.Scripts.Main;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class MadnessTorch : StsBossAncientsRelic, IRightCilckable
	{
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(CardKeyword.Exhaust)];
	public override RelicRarity Rarity => RelicRarity.Ancient;
	protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1)];

	[SavedProperty]
	public bool IsEnabled
	{
		get => _isEnabled;
		set
		{
			AssertMutable();
			_isEnabled = value;
			Status = _isEnabled ? RelicStatus.Normal : RelicStatus.Disabled;
		}
	}

	private bool _isEnabled = true;

	public Task OnRightClick(PlayerChoiceContext context)
	{
		IsEnabled = !IsEnabled;
		Flash();
		return Task.CompletedTask;
	}

	public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
	{
		if (!IsEnabled || card.Owner != Owner)
		{
			modifiedCost = originalCost;
			return false;
		}
		modifiedCost = 0m;
		return true;
	}

		public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
		{
			if (!IsEnabled)
			{
				return;
			}
			if (cardPlay.Card.Owner != Owner)
			{
				return;
			}
			await CardCmd.Exhaust(context, cardPlay.Card);
		}
	}
}
