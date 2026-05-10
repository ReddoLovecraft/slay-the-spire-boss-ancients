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
public class SlimedBoss : FirstFloorAncientModel
{
	// 选项按钮颜色
	public override Color ButtonColor => new Color(0.216f, 0.606f, 0.12f, 1.0f);
	// 对话框颜色
	public override Color DialogueColor => new Color(0.28f, 0.673f, 0.244f, 0.663f);
	// 自定义场景的路径
	public override string? CustomScenePath => "res://StsBossAncients/ArtWorks/Ancinets/Backgrounds/slimedboss.tscn";
	// 自定义地图图标和轮廓的路径
	public override string? CustomMapIconPath => "res://StsBossAncients/ArtWorks/Ancinets/slime.png";
	public override string? CustomMapIconOutlinePath => "res://StsBossAncients/ArtWorks/Ancinets/Outlines/slime.png";
	// 历史记录图标路径
	public override string? CustomRunHistoryIconPath => "res://StsBossAncients/ArtWorks/Ancinets/History/slime.png";
	public override string? CustomRunHistoryIconOutlinePath => "res://StsBossAncients/ArtWorks/Ancinets/History/slime.png";
	// 生成选项。每个选项有自己的池子。
	protected override OptionPools MakeOptionPools
	{
		get
		{
			RunState? state = RunManager.Instance.DebugOnlyGetState();
			var me = LocalContext.GetMe(state) ?? state?.Players.FirstOrDefault();

			int gluttonWeight = me?.Character is Ironclad ? 999 : 0;
			int corrosiveWeight = me?.Character is Silent ? 999 : 0;
			int heuristicWeight = me?.Character is Defect ? 999 : 0;
			int caneWeight = me?.Character is Regent ? 999 : 0;
			int topHatWeight = me?.Character is Necrobinder ? 999 : 0;
			int slimeBallWeight = (gluttonWeight + corrosiveWeight + heuristicWeight + caneWeight + topHatWeight) == 0 ? 999 : 0;

			var thirdPool = MakePool(
				AncientOption<GluttonHeart>(weight: gluttonWeight),
				AncientOption<VolatileCorrosiveAgent>(weight: corrosiveWeight),
				AncientOption<HeuristicAnalysis>(weight: heuristicWeight),
				AncientOption<RoyalGentlemanCane>(weight: caneWeight),
				AncientOption<SlimeTopHat>(weight: topHatWeight),
				AncientOption<SlimeBall>(weight: slimeBallWeight)
			);

			return new OptionPools(
				MakePool(
					AncientOption<MalignantTumor>(),
					AncientOption<ProliferationGeneFragment>()
				),
				MakePool(
					AncientOption<SpecialLubricant>(),
					AncientOption<NonNewtonianFluid>(),
					AncientOption<SlimeBall>()
				),
				thirdPool
			);
		}
	}
}

}
