using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.ValueProps;
using StsBossAncients.Scripts.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class RoyalGuard : StsBossAncientsRelic
	{
		protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<BufferPower>()];
		public override RelicRarity Rarity => RelicRarity.Ancient;

		public override async Task BeforeDamageReceived(PlayerChoiceContext choiceContext, Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
		{
			if (target != Owner.Creature)
			{
				return;
			}
			if (dealer == null || dealer.Side == target.Side)
			{
				return;
			}
			if (Owner.PlayerCombatState == null)
			{
				return;
			}
			if (!props.IsPoweredAttack())
			{
				return;
			}

			CardModel? attackCard = Owner.PlayerCombatState.Hand.Cards
				.Where(c => c.Type == CardType.Attack && c.TargetType == TargetType.AnyEnemy)
				.FirstOrDefault(c => c.IsValidTarget(dealer));

			if (attackCard == null)
			{
				return;
			}

			int intentValue = GetAttackerIntentTotalDamage(dealer);
			if (intentValue > 0)
			{
				DamageVar? dv = attackCard.DynamicVars?.Damage;
				if (dv != null)
				{
					dv.UpdateCardPreview(attackCard, CardPreviewMode.None, dealer, runGlobalHooks: true);
					int predicted = Math.Max(0, (int)dv.PreviewValue);
					if (predicted >= intentValue)
					{
						await PowerCmd.Apply<BufferPower>(Owner.Creature, 1, Owner.Creature, null);
					}
				}
			}

			Flash();
			await CardCmd.AutoPlay(choiceContext, attackCard, dealer);
		}

		private static int GetAttackerIntentTotalDamage(Creature attacker)
		{
			if (!attacker.IsMonster)
			{
				return 0;
			}
			var cs = attacker.CombatState;
			if (cs == null)
			{
				return 0;
			}
			var monster = attacker.Monster;
			if (monster == null || monster.NextMove == null || monster.NextMove.Intents == null)
			{
				return 0;
			}
			IEnumerable<Creature> targets = cs.GetOpponentsOf(attacker).Where(c => c.IsAlive);
			int total = 0;
			foreach (var intent in monster.NextMove.Intents)
			{
				if (intent is MegaCrit.Sts2.Core.MonsterMoves.Intents.AttackIntent ai)
				{
					total += ai.GetTotalDamage(targets, attacker);
				}
			}
			return Math.Max(0, total);
		}
	}
}
