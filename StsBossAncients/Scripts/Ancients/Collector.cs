using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Runs;
using StsBossAncicents;
using StsBossAncients.Scripts.Relics;
using System.Linq;

namespace StsBossAncients.Scripts.Ancients
{
public class Collector : SecondFloorAncientModel
{
	// 选项按钮颜色
	public override Color ButtonColor => new Color(0.503f, 1.0f, 0.246f, 0.533f);
	// 对话框颜色
	public override Color DialogueColor => new Color(0.4f, 0.773f, 0.129f, 0.69f);
	// 自定义场景的路径
	public override string? CustomScenePath => "res://StsBossAncients/ArtWorks/Ancinets/Backgrounds/collector.tscn";
	// 自定义地图图标和轮廓的路径
	public override string? CustomMapIconPath => "res://StsBossAncients/ArtWorks/Ancinets/collector.png";
	public override string? CustomMapIconOutlinePath => "res://StsBossAncients/ArtWorks/Ancinets/Outlines/collector.png";
	// 历史记录图标路径
	public override string? CustomRunHistoryIconPath => "res://StsBossAncients/ArtWorks/Ancinets/History/collector.png";
	public override string? CustomRunHistoryIconOutlinePath => "res://StsBossAncients/ArtWorks/Ancinets/History/collector.png";
	// 生成选项。每个选项有自己的池子。
	protected override OptionPools MakeOptionPools
	{
		get
		{
			RunState? state = RunManager.Instance.DebugOnlyGetState();
			var me = LocalContext.GetMe(state) ?? state?.Players.FirstOrDefault();

			int leigonWeight = me?.Character is Ironclad ? 999 : 0;
			int bladeWeight = me?.Character is Silent ? 999 : 0;
			int programWeight = me?.Character is Defect ? 999 : 0;
			int stARWeight = me?.Character is Regent ? 999 : 0;
			int scrollWeight = me?.Character is Necrobinder ? 999 : 0;
			int brainWeight = (leigonWeight + bladeWeight + programWeight + stARWeight + scrollWeight) == 0 ? 999 : 0;

			var thirdPool = MakePool(
				AncientOption<IroncladLegionCollection>(weight: leigonWeight),
				AncientOption<HuntressBlade>(weight: bladeWeight),
				AncientOption<FragmentOrganizerProgram>(weight: programWeight),
				AncientOption<GalaxyStar>(weight: stARWeight),
				AncientOption<DoomsdayScroll>(weight: scrollWeight),
				AncientOption<BrainInJar>(weight: brainWeight)
			);

			return new OptionPools(
				MakePool(
					AncientOption<ThornCrown>(),
					AncientOption<RuneCube>()
				),
				MakePool(
					AncientOption<TorchHeadDoll>(),
					AncientOption<HellfireScepter>(),
					AncientOption<BrainInJar>()
				),
				thirdPool
			);
		}
	}
	
}

}
