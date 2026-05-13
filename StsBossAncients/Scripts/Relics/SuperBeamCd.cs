using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using StsBossAncients.Scripts.Cards;
using StsBossAncients.Scripts.Main;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class SuperBeamCd : StsBossAncientsRelic
	{
		protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<TrueHyperbeam>()];
		public override RelicRarity Rarity => RelicRarity.Ancient;
		public override bool HasUponPickupEffect => true;
		

		public override async Task AfterObtained()
		{
			CardModel c = Owner.RunState.CreateCard<TrueHyperbeam>(Owner);
			await CardPileCmd.Add(c, PileType.Deck);
		}
	}
}
