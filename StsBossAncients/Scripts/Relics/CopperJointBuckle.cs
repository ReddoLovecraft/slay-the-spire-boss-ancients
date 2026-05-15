using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.ValueProps;
using StsBossAncients.Scripts.Main;
using System;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class CopperJointBuckle : StsBossAncientsRelic
{
	public override RelicRarity Rarity => RelicRarity.Ancient;
	private bool _didReduce;

	public override decimal ModifyHpLostAfterOsty(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
	{
		if (!CombatManager.Instance.IsInProgress)
		{
			return amount;
		}
		if (!StsBossAncientsReflection.IsLikelyOsty(target, Owner))
		{
			return amount;
		}

		decimal capped = Math.Min(5m, amount);
		_didReduce = capped != amount;
		return capped;
	}

	public override Task AfterModifyingHpLostAfterOsty()
	{
		if (_didReduce)
		{
			_didReduce = false;
			Flash();
		}
		return Task.CompletedTask;
	}
}

