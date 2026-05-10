using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.ValueProps;
using StsBossAncients.Scripts.Main;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class SlimeTopHat : StsBossAncientsRelic
	{
	public override RelicRarity Rarity => RelicRarity.Ancient;

		private int _pendingSplitDamage;
		private bool _hasPendingSplit;

		public override async Task BeforeCombatStart()
		{
			CombatState? combat = Owner.Creature.CombatState;
			if (combat == null)
			{
				return;
			}

			Flash();
			for (int i = 0; i < 3; i++)
			{
				MonsterModel monster = ModelDb.Monster<TwigSlimeM>().ToMutable();
				Creature pet = combat.CreateCreature(monster, Owner.Creature.Side, null);
				await PlayerCmd.AddPet(pet, Owner);
				pet.PrepareForNextTurn(combat.HittableEnemies, rollNewMove: false);
			}
		}

		public override decimal ModifyHpLostAfterOsty(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
		{
			if (!CombatManager.Instance.IsInProgress)
			{
				return amount;
			}
			if (target != Owner.Creature)
			{
				return amount;
			}
			if (amount <= 0m)
			{
				return amount;
			}
			if (!props.HasFlag(ValueProp.Move))
			{
				return amount;
			}

			List<Creature> slimes = GetLiveSlimes();
			if (slimes.Count == 0)
			{
				return amount;
			}

			int dmg = (int)Math.Floor(amount);
			if (dmg <= 0)
			{
				return amount;
			}

			_pendingSplitDamage += dmg;
			_hasPendingSplit = true;
			return 0m;
		}

		public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
		{
			if (!_hasPendingSplit)
			{
				return;
			}
			if (target != Owner.Creature)
			{
				return;
			}
			if (!CombatManager.Instance.IsInProgress)
			{
				_hasPendingSplit = false;
				_pendingSplitDamage = 0;
				return;
			}

			int dmg = _pendingSplitDamage;
			_hasPendingSplit = false;
			_pendingSplitDamage = 0;
			if (dmg <= 0)
			{
				return;
			}

			List<Creature> slimes = GetLiveSlimes();
			if (slimes.Count == 0)
			{
				return;
			}

			Flash();
			int per = dmg / slimes.Count;
			int rem = dmg % slimes.Count;
			for (int i = 0; i < slimes.Count; i++)
			{
				int share = per + ((i < rem) ? 1 : 0);
				if (share <= 0)
				{
					continue;
				}
				await CreatureCmd.Damage(choiceContext, slimes[i], share, ValueProp.Unblockable | ValueProp.Unpowered, dealer: null, cardSource: null);
			}
		}

		private List<Creature> GetLiveSlimes()
		{
			Creature ownerCreature = Owner.Creature;
			return ownerCreature.Pets
				.Where(p => p.IsAlive && p.PetOwner == Owner && p.Monster is TwigSlimeM)
				.ToList();
		}
	}
}
