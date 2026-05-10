using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using StsBossAncients.Scripts.Main;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class SoulBurnBlood : StsBossAncientsRelic
	{
	public override RelicRarity Rarity => RelicRarity.Ancient;
	protected override IEnumerable<IHoverTip> ExtraHoverTips => HoverTipFactory.FromRelic<BurningBlood>();
	public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
	{
		if (player != Owner)
		{
			return;
		}
		Flash();
		await CreatureCmd.Heal(Owner.Creature, 6);
	}

	public override async Task AfterRoomEntered(AbstractRoom room)
	{
		await CreatureCmd.LoseMaxHp(new ThrowingPlayerChoiceContext(), Owner.Creature, 1, false);
	}

		public override async Task AfterObtained()
		{
		RelicModel? burningBlood = Owner.Relics.FirstOrDefault(r =>
			string.Equals(r.Id.Entry, "BURNING_BLOOD", StringComparison.OrdinalIgnoreCase)
			|| string.Equals(r.Id.Entry, "BURNINGBLOOD", StringComparison.OrdinalIgnoreCase));

			if (burningBlood != null)
			{
				await RelicCmd.Remove(burningBlood);
			}
		}
	}
}
