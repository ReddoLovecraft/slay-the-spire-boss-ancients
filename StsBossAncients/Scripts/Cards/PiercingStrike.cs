using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Cards;

[Pool(typeof(QuestCardPool))]
public sealed class PiercingStrike : CustomCardModel
{
	 public override string PortraitPath => $"res://StsBossAncients/ArtWorks/Cards/{Id.Entry}.png";
	protected override IEnumerable<DynamicVar> CanonicalVars => new[] { new DamageVar(12m, ValueProp.Move) };

	public PiercingStrike()
		: base(1, CardType.Attack, CardRarity.Ancient, TargetType.AllEnemies)
	{
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		var owner = Owner;
		var ownerCreature = owner?.Creature;
		CombatState? cs = CombatState ?? ownerCreature?.CombatState;
		if (cs == null || ownerCreature == null)
		{
			return;
		}

		List<DamageResult> results = (await CreatureCmd.Damage(choiceContext, cs.HittableEnemies, DynamicVars.Damage, ownerCreature, this)).ToList();
		foreach (DamageResult r in results)
		{
			if (r.UnblockedDamage <= 0)
			{
				continue;
			}
			if (!r.Receiver.IsAlive)
			{
				continue;
			}
			await PowerCmd.Apply<PoisonPower>(r.Receiver, r.UnblockedDamage, ownerCreature, this);
		}
	}

	protected override void OnUpgrade()
	{
		DynamicVars.Damage.UpgradeValueBy(3m);
	}
}
