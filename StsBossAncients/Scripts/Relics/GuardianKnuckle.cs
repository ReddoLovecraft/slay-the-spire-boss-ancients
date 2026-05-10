using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.ValueProps;
using StsBossAncients.Scripts.Main;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class GuardianKnuckle : StsBossAncientsRelic
	{
	public override RelicRarity Rarity => RelicRarity.Ancient;

		public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
		{
			if (dealer == null || target == null)
			{
				return 0m;
			}
			if (dealer.Monster is not Osty)
			{
				return 0m;
			}
			if (dealer.PetOwner != Owner)
			{
				return 0m;
			}
			if (dealer.CurrentHp <= 0)
			{
				return 0m;
			}
			if (target.Side == Owner.Creature.Side)
			{
				return 0m;
			}
			if (amount <= 0m)
			{
				return 0m;
			}

			Flash();
			return dealer.CurrentHp;
		}
	}
}
