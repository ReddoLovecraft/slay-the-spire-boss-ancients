using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Orbs;
using MegaCrit.Sts2.Core.Models.RelicPools;
using StsBossAncients.Scripts.Main;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class GlassCore : StsBossAncientsRelic
	{
	public override RelicRarity Rarity => RelicRarity.Ancient;
	protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[2]
	{
		HoverTipFactory.Static(StaticHoverTip.Channeling),
		HoverTipFactory.FromOrb<GlassOrb>()
	};
		public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
		{
			if (player != Owner)
			{
				return;
			}

			Flash();
			await OrbCmd.Channel<GlassOrb>(new ThrowingPlayerChoiceContext(), base.Owner);
			return;
		}
	}
}
