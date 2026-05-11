using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.RelicPools;
using StsBossAncients.Scripts.Main;
using System.Linq;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class IroncladLegionCollection : StsBossAncientsRelic
	{
		public override RelicRarity Rarity => RelicRarity.Ancient;

		public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
		{
			if (card.Owner != Owner)
			{
				return playCount;
			}
			if (card.Tags.Contains(CardTag.Strike))
			{
				return playCount + 1;
			}
			return playCount;
		}
	}
}
