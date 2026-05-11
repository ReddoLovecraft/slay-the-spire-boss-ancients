using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Saves;
using StsBossAncients.Scripts.Main;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class NanobotMud : StsBossAncientsRelic
	{
		public override RelicRarity Rarity => RelicRarity.Ancient;

		[SavedProperty]
		public int LastTurnStartHp
		{
			get => _lastTurnStartHp;
			set
			{
				AssertMutable();
				_lastTurnStartHp = value;
			}
		}
		private int _lastTurnStartHp;

		public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
		{
			if (player != Owner)
			{
				return;
			}
			int current = (int)Owner.Creature.CurrentHp;
			if (LastTurnStartHp > 0 && current < LastTurnStartHp)
			{
				Flash();
				await CreatureCmd.SetCurrentHp(Owner.Creature, LastTurnStartHp);
			}
			LastTurnStartHp = (int)Owner.Creature.CurrentHp;
		}
	}
}
