using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Saves.Runs;
using StsBossAncients.Scripts.Main;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class SixRealmsReincarnation : StsBossAncientsRelic
	{
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.ForEnergy(this)];
	public override RelicRarity Rarity => RelicRarity.Ancient;

	[SavedProperty]
	public int MaxEnergyBonus
	{
		get => _maxEnergyBonus;
		set
		{
			AssertMutable();
			_maxEnergyBonus = value;
			DynamicVars.Energy.BaseValue = value;
			InvokeDisplayAmountChanged();
		}
	}

	[SavedProperty]
	public int TurnCounter
	{
		get => _turnCounter;
		set
		{
			AssertMutable();
			_turnCounter = value;
			InvokeDisplayAmountChanged();
		}
	}

	private int _turnCounter;
	private int _maxEnergyBonus;
	protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar("Energy", 0), new EnergyVar("IncEnergy", 1)];
	public override bool ShowCounter => true;
	public override int DisplayAmount => TurnCounter % 6;

	public override decimal ModifyMaxEnergy(Player player, decimal amount)
	{
		if (player != Owner)
		{
			return amount;
		}
		return amount + MaxEnergyBonus;
	}

		public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
		{
			if (player != Owner)
			{
				return Task.CompletedTask;
			}

			TurnCounter++;
			if (TurnCounter % 6 != 0)
			{
				return Task.CompletedTask;
			}

			Flash();
			MaxEnergyBonus += DynamicVars["IncEnergy"].IntValue;
			return Task.CompletedTask;
		}
	}
}
