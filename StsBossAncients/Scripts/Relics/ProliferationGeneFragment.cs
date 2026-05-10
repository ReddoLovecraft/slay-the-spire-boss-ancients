using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using StsBossAncients.Scripts.Enchantments;
using StsBossAncients.Scripts.Main;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class ProliferationGeneFragment : StsBossAncientsRelic
	{
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [..HoverTipFactory.FromEnchantment<ProliferationEnchantment>(1)];
	public override RelicRarity Rarity => RelicRarity.Ancient;

		public override async Task AfterObtained()
		{
			CardModel? picked = (await CardSelectCmd.FromDeckGeneric(prefs: new CardSelectorPrefs(SelectionScreenPrompt, 1), player: Owner)).FirstOrDefault();
			if (picked != null)
			{
				CardCmd.Enchant<ProliferationEnchantment>(picked, 1m);
			}
		}
	}
}
