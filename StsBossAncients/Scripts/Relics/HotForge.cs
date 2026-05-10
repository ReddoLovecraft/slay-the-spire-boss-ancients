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
	public sealed class HotForge : StsBossAncientsRelic
	{
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [..HoverTipFactory.FromEnchantment<ScorchingEnchantment>(1)];
	public override RelicRarity Rarity => RelicRarity.Ancient;

		public override async Task AfterObtained()
		{
			CardModel? picked = (await CardSelectCmd.FromDeckGeneric(
				player: Owner,
				prefs: new CardSelectorPrefs(SelectionScreenPrompt, 1),
				filter: c => c.Type == CardType.Attack
			)).FirstOrDefault();
			if (picked != null)
			{
				Flash();
				CardCmd.Enchant<ScorchingEnchantment>(picked, 1m);
			}
		}
	}
}
