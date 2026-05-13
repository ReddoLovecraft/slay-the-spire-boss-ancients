using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using StsBossAncients.Scripts.Main;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.HoverTips;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class FragmentOrganizerProgram : StsBossAncientsRelic
	{
		public override RelicRarity Rarity => RelicRarity.Ancient;
			protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
	{
		HoverTipFactory.FromCard<Defragment>()
	});

		public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
		{
			if (player != Owner)
			{
				return;
			}
			Flash();
			CardModel card = Owner.Creature.CombatState.CreateCard<Defragment>(Owner);
			await CardPileCmd.AddGeneratedCardsToCombat([card], PileType.Hand, addedByPlayer: true);
		}
	}
}
