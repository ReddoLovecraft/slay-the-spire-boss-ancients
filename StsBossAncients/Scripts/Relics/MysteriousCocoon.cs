using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.ValueProps;
using StsBossAncients.Scripts.Cards;
using StsBossAncients.Scripts.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class MysteriousCocoon : StsBossAncientsRelic
{
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Fatal), HoverTipFactory.FromCard<TrainingStrike>(false), HoverTipFactory.FromCard<PiercingStrike>(false)];
	public override RelicRarity Rarity => RelicRarity.Ancient;
	public override bool HasUponPickupEffect => true;
	public override bool ShowCounter => true;
	public override int DisplayAmount => Experience;

	[SavedProperty]
	public int Experience
	{
		get => _experience;
		set
		{
			AssertMutable();
			_experience = value;
			InvokeDisplayAmountChanged();
		}
	}

	[SavedProperty]
	public bool HasTransformed
	{
		get => _hasTransformed;
		set
		{
			AssertMutable();
			_hasTransformed = value;
		}
	}

	private int _experience;
	private bool _hasTransformed;

	public override async Task AfterObtained()
	{
		Experience = 0;
		HasTransformed = false;

		List<CardModel> strikes = Owner.Deck.Cards.Where(IsBasicStrike).ToList();
		if (strikes.Count > 0)
		{
			await CardPileCmd.RemoveFromDeck(strikes, showPreview: true);
		}

		for (int i = 0; i < 5; i++)
		{
			CardModel c = Owner.RunState.CreateCard<TrainingStrike>(Owner);
			await CardPileCmd.Add(c, PileType.Deck);
		}
	}

	public override Task AfterCombatVictory(CombatRoom room)
	{
		if (!HasTransformed && Experience >= 10)
		{
			return TransformTrainingStrikes();
		}
		return Task.CompletedTask;
	}

	private static bool IsBasicStrike(CardModel c)
	{
		return c.Rarity == CardRarity.Basic && c.Tags.Contains(CardTag.Strike);
	}

	private async Task TransformTrainingStrikes()
	{
		List<TrainingStrike> training = Owner.Deck.Cards.OfType<TrainingStrike>().ToList();
		if (training.Count == 0)
		{
			HasTransformed = true;
			return;
		}

		List<bool> upgraded = training.Select(c => c.IsUpgraded).ToList();
		await CardPileCmd.RemoveFromDeck(training.Cast<CardModel>().ToList(), showPreview: true);

		for (int i = 0; i < upgraded.Count; i++)
		{
			CardModel c = Owner.RunState.CreateCard<PiercingStrike>(Owner);
			if (upgraded[i])
			{
				CardCmd.Upgrade(c);
			}
			await CardPileCmd.Add(c, PileType.Deck);
		}

		HasTransformed = true;
		Flash();
	}
}
