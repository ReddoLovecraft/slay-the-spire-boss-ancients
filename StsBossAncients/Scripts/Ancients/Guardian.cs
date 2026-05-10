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
public class Guardian : FirstFloorAncientModel
{
	// 选项按钮颜色
	public override Color ButtonColor => new Color(0.872f, 0.731f, 0.416f, 0.702f);
	// 对话框颜色
	public override Color DialogueColor => new Color(0.643f, 0.569f, 0.333f, 0.741f);
	// 自定义场景的路径
	public override string? CustomScenePath => "res://StsBossAncients/ArtWorks/Ancinets/Backgrounds/guardian.tscn";
	// 自定义地图图标和轮廓的路径
	public override string? CustomMapIconPath => "res://StsBossAncients/ArtWorks/Ancinets/guardian.png";
	public override string? CustomMapIconOutlinePath => "res://StsBossAncients/ArtWorks/Ancinets/Outlines/guardian.png";
	// 历史记录图标路径
	public override string? CustomRunHistoryIconPath => "res://StsBossAncients/ArtWorks/Ancinets/History/guardian.png";
	public override string? CustomRunHistoryIconOutlinePath => "res://StsBossAncients/ArtWorks/Ancinets/History/guardian.png";
	// 生成选项。每个选项有自己的池子。
	protected override OptionPools MakeOptionPools
	{
		get
		{
			RunState? state = RunManager.Instance.DebugOnlyGetState();
			var me = LocalContext.GetMe(state) ?? state?.Players.FirstOrDefault();

			int bladeWeight = me?.Character is Ironclad ? 999 : 0;
			int ironWeight = me?.Character is Silent ? 999 : 0;
			int glassWeight = me?.Character is Defect ? 999 : 0;
			int lastWeight = me?.Character is Regent ? 999 : 0;
			int knuckleWeight = me?.Character is Necrobinder ? 999 : 0;
			int engineWeight = (bladeWeight + ironWeight + glassWeight + lastWeight + knuckleWeight) == 0 ? 999 : 0;

			var thirdPool = MakePool(
				AncientOption<TwinCycloneBlades>(weight: bladeWeight),
				AncientOption<IronCaltrops>(weight: ironWeight),
				AncientOption<GlassCore>(weight: glassWeight),
				AncientOption<LastMachiavelli>(weight: lastWeight),
				AncientOption<GuardianKnuckle>(weight: knuckleWeight),
				AncientOption<SteamEngine>(weight: engineWeight)
			);

			return new OptionPools(
				MakePool(
					AncientOption<CopperScaleStack>(),
					AncientOption<AutomatedDefenseOverhaul>()
				),
				MakePool(
					AncientOption<MechanicalHunt>(),
					AncientOption<CapacitorDevice>(),
					AncientOption<SteamEngine>()
				),
				thirdPool
			);
		}
	}
	
}

}
