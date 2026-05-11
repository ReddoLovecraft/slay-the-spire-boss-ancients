using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.RelicPools;
using StsBossAncients.Scripts.Enchantments;
using StsBossAncients.Scripts.Main;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class RitualDagger : StsBossAncientsRelic
{
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [.. HoverTipFactory.FromEnchantment<SacrificeEnchantment>(1)];
	public override RelicRarity Rarity => RelicRarity.Ancient;

	public override Task AfterObtained()
	{
		bool triggered = false;
		foreach (CardModel c in Owner.Deck.Cards)
		{
			if (c.Type != CardType.Attack)
			{
				continue;
			}
			if (c.Enchantment != null)
			{
				continue;
			}

			CardCmd.Enchant<SacrificeEnchantment>(c, 1m);
			triggered = true;
		}

		if (triggered)
		{
			Flash();
		}
		return Task.CompletedTask;
	}
}
