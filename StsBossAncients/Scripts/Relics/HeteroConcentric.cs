using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.RelicPools;
using StsBossAncients.Scripts.Enchantments;
using StsBossAncients.Scripts.Main;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class HeteroConcentric : StsBossAncientsRelic
{
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [.. HoverTipFactory.FromEnchantment<OneHeartEnchantment>(1)];
	public override RelicRarity Rarity => RelicRarity.Ancient;

	public override async Task AfterObtained()
	{
		var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 2, 2)
		{
			RequireManualConfirmation = true,
			Cancelable = false
		};

		var selected = (await CardSelectCmd.FromDeckGeneric(
			player: Owner,
			prefs: prefs,
			filter: c => c.Type != CardType.Power && c.Enchantment == null
		)).ToList();

		if (selected.Count == 0)
		{
			return;
		}

		Flash();
		foreach (CardModel c in selected)
		{
			CardCmd.Enchant<OneHeartEnchantment>(c, 1m);
		}
	}
}
