using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Models.RelicPools;
using Patchouib.Scrpits.Main;
using StsBossAncients.Scripts.Enchantments;
using StsBossAncients.Scripts.Main;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class FlameDagger : StsBossAncientsRelic
	{
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [..HoverTipFactory.FromEnchantment<FireEnchantment>(1), HoverTipFactory.FromPower<IgnitePower>(),HoverTipFactory.FromCard<Shiv>()];
	public override RelicRarity Rarity => RelicRarity.Ancient;

		public override Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? source)
		{
		if (!CombatManager.Instance.IsInProgress)
		{
			return Task.CompletedTask;
		}
		if (card.Owner != Owner)
		{
			return Task.CompletedTask;
		}
		if (card.Pile?.Type != PileType.Hand || oldPileType == PileType.Hand)
		{
			return Task.CompletedTask;
		}

		string entry = card.Id.Entry ?? string.Empty;
		if (!string.Equals(entry, "SHIV", StringComparison.OrdinalIgnoreCase)
			&& !entry.Contains("SHIV", StringComparison.OrdinalIgnoreCase)
			&& !entry.Contains("KNIFE", StringComparison.OrdinalIgnoreCase))
		{
			return Task.CompletedTask;
		}
		if (card.Enchantment is Inky)
		{
			return Task.CompletedTask;
		}
		if (card.Enchantment != null)
		{
			return Task.CompletedTask;
		}

			Flash();
			CardCmd.Enchant<FireEnchantment>(card, 1m);
			return Task.CompletedTask;
		}
	}
}
