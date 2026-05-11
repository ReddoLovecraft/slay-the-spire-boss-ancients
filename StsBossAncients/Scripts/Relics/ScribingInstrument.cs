using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using StsBossAncients.Scripts.Main;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class ScribingInstrument : StsBossAncientsRelic
{
	public override RelicRarity Rarity => RelicRarity.Ancient;

	public override bool TryModifyRestSiteOptions(Player player, ICollection<RestSiteOption> options)
	{
		if (player != Owner)
		{
			return false;
		}
		if (options.Any(o => o is ScribingInstrumentCopyOption))
		{
			return false;
		}
		if (GetEligibleRelics().Count == 0)
		{
			return false;
		}

		options.Add(new ScribingInstrumentCopyOption(player));
		return true;
	}

	private List<RelicModel> GetEligibleRelics()
	{
		return Owner.Relics
			.Where(r => r != this && !r.IsMelted && r.Status != RelicStatus.Disabled)
			.ToList();
	}

	private async Task CopyFlow()
	{
		List<RelicModel> candidates = GetEligibleRelics();
		if (candidates.Count == 0)
		{
			return;
		}

		RelicModel? selected = await RelicSelectCmd.FromChooseARelicScreen(Owner, candidates);
		if (selected == null)
		{
			return;
		}

		RelicModel copy = ModelDb.GetById<RelicModel>(selected.Id).ToMutable();
		await RelicCmd.Obtain(copy, Owner);
	}

	private sealed class ScribingInstrumentCopyOption : RestSiteOption
	{
		public override string OptionId => "SCRIBING_INSTRUMENT_COPY";

		public ScribingInstrumentCopyOption(Player owner)
			: base(owner)
		{
		}

		public override async Task<bool> OnSelect()
		{
			ScribingInstrument? relic = Owner.Relics.OfType<ScribingInstrument>().FirstOrDefault();
			if (relic == null)
			{
				return false;
			}

			relic.Flash();
			await relic.CopyFlow();
			return true;
		}
	}
}

