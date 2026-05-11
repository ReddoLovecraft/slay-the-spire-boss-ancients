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
public sealed class Phonograph : StsBossAncientsRelic
{
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [.. HoverTipFactory.FromEnchantment<EchoEnchantment>(1)];
	public override RelicRarity Rarity => RelicRarity.Ancient;

	public override async Task AfterObtained()
	{
		var picked = (await CardSelectCmd.FromDeckGeneric(
			player: Owner,
			prefs: new CardSelectorPrefs(SelectionScreenPrompt, 1, 1)
			{
				RequireManualConfirmation = true,
				Cancelable = false
			},
			filter: c => c.Type != CardType.Power && c.Enchantment == null
		)).FirstOrDefault();

		if (picked == null)
		{
			return;
		}

		Flash();
		CardCmd.Enchant<EchoEnchantment>(picked, 1m);
	}
}
