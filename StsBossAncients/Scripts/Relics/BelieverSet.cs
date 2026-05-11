using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using Patchoulib.Scrpits.Main;
using StsBossAncients.Scripts.Cards;
using StsBossAncients.Scripts.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class BelieverSet : StsBossAncientsRelic
{
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<RitualPower>(), HoverTipFactory.FromCard<RitualStrike>(false), HoverTipFactory.FromCard<Chant>(false)];
	public override RelicRarity Rarity => RelicRarity.Ancient;
	public override bool HasUponPickupEffect => true;

	public override async Task AfterObtained()
	{
		List<CardModel> strikes = Owner.Deck.Cards.Where(IsBasicStrike).ToList();
		List<CardModel> defends = Owner.Deck.Cards.Where(IsBasicDefend).ToList();

		if (strikes.Count + defends.Count == 0)
		{
			return;
		}

		if (strikes.Count > 0)
		{
			await CardPileCmd.RemoveFromDeck(strikes, showPreview: true);
		}
		if (defends.Count > 0)
		{
			await CardPileCmd.RemoveFromDeck(defends, showPreview: true);
		}

		for (int i = 0; i < strikes.Count; i++)
		{
			CardModel c = ModelDb.Card<RitualStrike>().ToMutable();
			c.Owner = Owner;
			await CardPileCmd.Add(c, PileType.Deck, CardPilePosition.Bottom, null);
		}

		for (int i = 0; i < defends.Count; i++)
		{
			CardModel c = ModelDb.Card<Chant>().ToMutable();
			c.Owner = Owner;
			await CardPileCmd.Add(c, PileType.Deck, CardPilePosition.Bottom, null);
		}
	}

	public override async Task BeforeCombatStart()
	{
        Tools.Talk("咔——咔！", Owner.Creature);
		foreach (Creature enemy in Owner.Creature.CombatState.HittableEnemies.ToList())
		{
			if (!enemy.IsAlive || enemy.Monster == null)
			{
				continue;
			}
			if (!IsBirdCultEnemy(enemy.Monster.Id.Entry))
			{
				continue;
			}

			Flash();
			await CreatureCmd.Escape(enemy, removeCreatureNode: true);
		}
	}

	private static bool IsBasicStrike(CardModel c)
	{
		return c.Rarity == CardRarity.Basic && c.Tags.Contains(CardTag.Strike);
	}

	private static bool IsBasicDefend(CardModel c)
	{
		return c.Rarity == CardRarity.Basic && c.Tags.Contains(CardTag.Defend);
	}

	private static bool IsBirdCultEnemy(string monsterEntry)
	{
		if (monsterEntry.Contains("cult", StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}
		if (monsterEntry.Contains("cultist", StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}
		if (monsterEntry.Contains("byrd", StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}
		if (monsterEntry.Contains("bird", StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}
		return false;
	}
}
