using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using StsBossAncients.Scripts.Main;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class BrokenCrown : StsBossAncientsRelic
	{
		protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.ForEnergy(this)];
		protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1)];
		public override RelicRarity Rarity => RelicRarity.Ancient;
		public override bool TryModifyCardRewardOptions(Player player, List<CardCreationResult> cardRewardOptions, CardCreationOptions creationOptions)
		{
			if (player != Owner)
			{
				return false;
			}
			if (cardRewardOptions.Count <= 1)
			{
				return false;
			}
			this.Flash();
			cardRewardOptions.RemoveAt(cardRewardOptions.Count - 1);
			return true;
		}
		  public override decimal ModifyMaxEnergy(Player player, decimal amount)
        {
            if (player != base.Owner)
            {
                return amount;
            }
            return amount + 1;
        }
	}
}
