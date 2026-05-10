using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.RelicPools;
using StsBossAncients.Scripts.Main;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class SteamEngine : StsBossAncientsRelic
	{
	protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1)];
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.ForEnergy(this), HoverTipFactory.FromCard<Burn>(false)];
	public override RelicRarity Rarity => RelicRarity.Ancient;

		public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
		{
			if (player != Owner)
			{
				return;
			}

			Flash();
			await CardPileCmd.AddToCombatAndPreview<Burn>(Owner.Creature, PileType.Draw, 1, addedByPlayer: true);
			await PlayerCmd.GainEnergy(1, Owner);
		}
	}
}
