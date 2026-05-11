using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Cards
{
	[Pool(typeof(CurseCardPool))]
	public sealed class Pain : CustomCardModel
	{
		 public override string PortraitPath => $"res://ArtWorks/Cards/{Id.Entry}.png";
		public override int MaxUpgradeLevel => 0;
		public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Unplayable };

		public Pain()
			: base(-1, CardType.Curse, CardRarity.Curse, TargetType.None)
		{
		}

		public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
		{
			if (Owner?.PlayerCombatState == null)
			{
				return;
			}
			if (Pile?.Type != PileType.Hand)
			{
				return;
			}
			if (cardPlay.Card == this)
			{
				return;
			}
			if (cardPlay.Card?.Owner != Owner)
			{
				return;
			}

			await CreatureCmd.Damage(choiceContext, Owner.Creature, 1, ValueProp.Unblockable | ValueProp.Unpowered, null, this);
		}
	}
}
