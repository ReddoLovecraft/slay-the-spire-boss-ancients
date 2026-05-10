using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Saves.Runs;
using StsBossAncients.Scripts.Main;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class SlimeBall : StsBossAncientsRelic
	{
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [..HoverTipFactory.FromEnchantment<Steady>(1)];
	public override RelicRarity Rarity => RelicRarity.Ancient;

		public override async Task AfterObtained()
		{
			CardSelectorPrefs prefs = new CardSelectorPrefs(SelectionScreenPrompt, 0, 7)
			{
				Cancelable = true
			};
			List<CardModel> picked = (await CardSelectCmd.FromDeckGeneric(Owner, prefs)).ToList();
			foreach (CardModel card in picked)
			{
				CardCmd.Enchant<Steady>(card, 1m);
			}
		}
	}
}
