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
public class HexaGhost : FirstFloorAncientModel
{
	// 选项按钮颜色
	public override Color ButtonColor => new Color(0.925f, 0.486f, 0.965f, 0.643f);
	// 对话框颜色
	public override Color DialogueColor => new Color(0.759f, 0.349f, 0.967f, 0.663f);
	// 自定义场景的路径
	public override string? CustomScenePath => "res://StsBossAncients/ArtWorks/Ancinets/Backgrounds/hexaghost.tscn";
	// 自定义地图图标和轮廓的路径
	public override string? CustomMapIconPath => "res://StsBossAncients/ArtWorks/Ancinets/hexaghost.png";
	public override string? CustomMapIconOutlinePath => "res://StsBossAncients/ArtWorks/Ancinets/Outlines/hexaghost.png";
	// 历史记录图标路径
	public override string? CustomRunHistoryIconPath => "res://StsBossAncients/ArtWorks/Ancinets/History/hexaghost.png";
	public override string? CustomRunHistoryIconOutlinePath => "res://StsBossAncients/ArtWorks/Ancinets/History/hexaghost.png";
	// 生成选项。每个选项有自己的池子。
	protected override OptionPools MakeOptionPools
	{
		get
		{
			RunState? state = RunManager.Instance.DebugOnlyGetState();
			var me = LocalContext.GetMe(state) ?? state?.Players.FirstOrDefault();

			int bloodWeight = me?.Character is Ironclad ? 999 : 0;
			int daggerWeight = me?.Character is Silent ? 999 : 0;
			int moudleWeight = me?.Character is Defect ? 999 : 0;
			int sixWeight = me?.Character is Regent ? 999 : 0;
			int furyWeight = me?.Character is Necrobinder ? 999 : 0;
			int flameFlowerWeight = (bloodWeight + daggerWeight + moudleWeight + sixWeight + furyWeight) == 0 ? 999 : 0;

			var thirdPool = MakePool(
				AncientOption<SoulBurnBlood>(weight: bloodWeight),
				AncientOption<FlameDagger>(weight: daggerWeight),
				AncientOption<FlameSprayerModule>(weight: moudleWeight),
				AncientOption<SixRealmsReincarnation>(weight: sixWeight),
				AncientOption<WraithFury>(weight: furyWeight),
				AncientOption<FlameFlowerPot>(weight: flameFlowerWeight)
			);

			return new OptionPools(
				MakePool(
					AncientOption<HotForge>(),
					AncientOption<HellfireFirstStrike>()
				),
				MakePool(
					AncientOption<HellfireLantern>(),
					AncientOption<MadnessTorch>(),
					AncientOption<FlameFlowerPot>()
				),
				thirdPool
			);
		}
	}
}

}
