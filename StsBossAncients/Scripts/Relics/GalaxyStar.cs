using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Entities.Relics;
using Godot;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.PotionPools;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using StsBossAncients.Scripts.Main;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class GalaxyStar : StsBossAncientsRelic
{
	public override RelicRarity Rarity => RelicRarity.Ancient;

	public override async Task AfterRoomEntered(AbstractRoom room)
	{
		if (Owner.Creature.IsDead)
		{
			return;
		}
		if (Owner.Gold < 30)
		{
			return;
		}

		Flash();
		await PlayerCmd.LoseGold(30, Owner, GoldLossType.Spent);

		int roll = Owner.PlayerRng.Rewards.NextInt(11);

		if (room is CombatRoom combatRoom)
		{
			if (roll == 0)
			{
				await PlayerCmd.GainGold(800, Owner);
				return;
			}
			if (roll == 1)
			{
				await PlayerCmd.GainGold(80, Owner);
				return;
			}

			Reward reward = CreateRewardForCombatRoomEnd(roll);
			reward.SetRng(Owner.PlayerRng.Rewards);
			combatRoom.AddExtraReward(Owner, reward);
			return;
		}

		if (roll == 0)
		{
			await PlayerCmd.GainGold(800, Owner);
			return;
		}
		if (roll == 1)
		{
			await PlayerCmd.GainGold(80, Owner);
			return;
		}

		Reward roomReward = CreateRewardForCombatRoomEnd(roll);
		roomReward.SetRng(Owner.PlayerRng.Rewards);
		_ = TaskHelper.RunSafely(OfferRewardForNonCombatRoom(roomReward));
	}

	private Reward CreateRewardForCombatRoomEnd(int roll)
	{
		return roll switch
		{
			2 => new CardRemovalReward(Owner),
			3 => CreateCardReward(CardRarity.Common),
			4 => CreateCardReward(CardRarity.Rare),
			5 => new PotionReward(CreatePotion(PotionRarity.Rare), Owner),
			6 => new PotionReward(CreatePotion(PotionRarity.Uncommon), Owner),
			7 => new PotionReward(CreatePotion(PotionRarity.Common), Owner),
			8 => new RelicReward(RelicRarity.Rare, Owner),
			9 => new RelicReward(RelicRarity.Uncommon, Owner),
			_ => new RelicReward(RelicRarity.Common, Owner)
		};
	}

	private async Task OfferRewardForNonCombatRoom(Reward reward)
	{
		if (Owner.Creature.IsDead)
		{
			return;
		}
		if (NRun.Instance == null)
		{
			return;
		}

		await NRun.Instance.ToSignal(NRun.Instance.GetTree(), SceneTree.SignalName.ProcessFrame);
		await NRun.Instance.ToSignal(NRun.Instance.GetTree(), SceneTree.SignalName.ProcessFrame);

		await new RewardsSet(Owner)
			.WithCustomRewards(new List<Reward> { reward })
			.WithSkippingDisallowed()
			.Offer();
	}

	private CardReward CreateCardReward(CardRarity rarity)
	{
		CardCreationOptions options = new CardCreationOptions(
				new[] { Owner.Character.CardPool },
				CardCreationSource.Other,
				CardRarityOddsType.Uniform,
				c => c.Rarity == rarity
			)
			.WithRngOverride(Owner.PlayerRng.Rewards);

		return new CardReward(options, 3, Owner)
		{
			CanSkip = false
		};
	}

	private PotionModel CreatePotion(PotionRarity rarity)
	{
		IEnumerable<PotionModel> options = Owner.Character.PotionPool.GetUnlockedPotions(Owner.UnlockState)
			.Concat(ModelDb.PotionPool<SharedPotionPool>().GetUnlockedPotions(Owner.UnlockState))
			.Where(p => p.Rarity == rarity);

		PotionModel? picked = Owner.PlayerRng.Rewards.NextItem(options.ToList());
		return (picked ?? ModelDb.PotionPool<SharedPotionPool>().GetUnlockedPotions(Owner.UnlockState).First()).ToMutable();
	}
}
