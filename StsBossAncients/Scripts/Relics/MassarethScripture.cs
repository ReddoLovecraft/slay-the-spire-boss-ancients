using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Saves;
using StsBossAncients.Scripts.Main;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class MassarethScripture : StsBossAncientsRelic
{
	public override RelicRarity Rarity => RelicRarity.Ancient;
	public override bool ShowCounter => true;
	public override int DisplayAmount => RitualCount;
	//protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<RitualPower>()];

	[SavedProperty]
	public int RitualCount
	{
		get => _ritualCount;
		set
		{
			AssertMutable();
			_ritualCount = value;
			InvokeDisplayAmountChanged();
		}
	}

	private int _ritualCount;

	public override Task BeforeCombatStart()
	{
		if (RitualCount > 0)
		{
			_ = PowerCmd.Apply<RitualPower>(Owner.Creature, RitualCount, Owner.Creature, null);
		}
		return Task.CompletedTask;
	}

	public override bool TryModifyRestSiteOptions(Player player, ICollection<RestSiteOption> options)
	{
		if (player != Owner)
		{
			return false;
		}
		if (options.Any(o => o is MassarethRitualOption))
		{
			return false;
		}
		options.Add(new MassarethRitualOption(player));
		return true;
	}

	private sealed class MassarethRitualOption : RestSiteOption
	{
		public override string OptionId => "MASSARETH_RITUAL";

		public MassarethRitualOption(Player owner) : base(owner)
		{
		}

		public override Task<bool> OnSelect()
		{
			MassarethScripture? relic = Owner.Relics.OfType<MassarethScripture>().FirstOrDefault();
			if (relic == null)
			{
				return Task.FromResult(false);
			}
			relic.Flash();
			relic.RitualCount += 6;
			return Task.FromResult(true);
		}
	}
}
