using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.RelicPools;
using StsBossAncients.Scripts.Enchantments;
using StsBossAncients.Scripts.Main;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class BrainInJar : StsBossAncientsRelic
	{
		protected override IEnumerable<IHoverTip> ExtraHoverTips => HoverTipFactory.FromEnchantment<MadnessEnchantment>(1);
		public override RelicRarity Rarity => RelicRarity.Ancient;
		public override bool HasUponPickupEffect => true;

		public override async Task AfterObtained()
		{
			CardSelectorPrefs prefs = new CardSelectorPrefs(new LocString("relics", $"{Id.Entry}.selectionScreenPrompt"), 2, 2)
			{
				RequireManualConfirmation = true,
				Cancelable = false
			};

			List<CardModel> picked = (await CardSelectCmd.FromDeckGeneric(Owner, prefs, null, null)).ToList();
			foreach (CardModel c in picked)
			{
				CardCmd.Enchant<MadnessEnchantment>(c, 1m);
			}
		}
	}
}
