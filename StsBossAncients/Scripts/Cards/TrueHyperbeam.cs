using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using StsBossAncients.Scripts.Powers;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Helpers;

namespace StsBossAncients.Scripts.Cards
{
	[Pool(typeof(EventCardPool))]
	public sealed class TrueHyperbeam : CustomCardModel
	{
		 public override string PortraitPath => $"res://StsBossAncients/ArtWorks/Cards/{Id.Entry}.png";
		protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(34m, ValueProp.Move),new EnergyVar(3)];

		public TrueHyperbeam()
			: base(0, CardType.Attack, CardRarity.Ancient, TargetType.AllEnemies)
		{
		}

		protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
		{
			await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).TargetingAllOpponents(base.CombatState)
			.WithAttackerAnim("Cast", 0.5f)
			.BeforeDamage(async delegate
			{
				List<Creature> enemies = base.CombatState.Enemies.Where((Creature e) => e.IsAlive).ToList();
				NHyperbeamVfx nHyperbeamVfx = NHyperbeamVfx.Create(base.Owner.Creature, enemies.Last());
				if (nHyperbeamVfx != null)
				{
					NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(nHyperbeamVfx);
					await Cmd.Wait(0.5f);
				}
				foreach (Creature item in enemies)
				{
					NHyperbeamImpactVfx nHyperbeamImpactVfx = NHyperbeamImpactVfx.Create(base.Owner.Creature, item);
					if (nHyperbeamImpactVfx != null)
					{
						NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(nHyperbeamImpactVfx);
					}
				}
			})
			.Execute(choiceContext);
			await PowerCmd.Apply<LoseEnergyNextTurnPower>(base.Owner.Creature, 3, base.Owner.Creature, this);
			if (!IsUpgraded)
			{
				PlayerCmd.EndTurn(base.Owner, canBackOut: false);
			}
		}

		protected override void OnUpgrade()
		{
		}
	}
}
