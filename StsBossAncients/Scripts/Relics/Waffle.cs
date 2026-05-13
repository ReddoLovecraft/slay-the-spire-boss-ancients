using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Models.RelicPools;
using StsBossAncients.Scripts.Main;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class Waffle : StsBossAncientsRelic
{
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [.. HoverTipFactory.FromEnchantment<Glam>(1)];
	public override RelicRarity Rarity => RelicRarity.Ancient;

	public override Task AfterObtained()
	{
		bool triggered = false;
		foreach (CardModel c in Owner.Deck.Cards)
		{
			if (c.Enchantment != null && c.Enchantment is not Glam)
			{
				continue;
			}
			if (c.Enchantment is Glam)
			{
				continue;
			}

			try
			{
				CardCmd.Enchant<Glam>(c, 1m);
				triggered = true;
			}
			catch
			{
			}
		}

		if (triggered)
		{
			Flash();
		}
		return Task.CompletedTask;
	}
}
