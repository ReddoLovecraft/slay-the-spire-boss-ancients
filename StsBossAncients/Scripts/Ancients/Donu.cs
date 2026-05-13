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
public class Donu : ThirdFloorAncientModel
{
	// 选项按钮颜色
	public override Color ButtonColor => new Color(0.98f, 0.78f, 0.408f, 0.612f);
	// 对话框颜色
	public override Color DialogueColor => new Color(0.868f, 0.66f, 0.285f, 0.784f);
	// 自定义场景的路径
	public override string? CustomScenePath => "res://StsBossAncients/ArtWorks/Ancinets/Backgrounds/donu.tscn";
	// 自定义地图图标和轮廓的路径
	public override string? CustomMapIconPath => "res://StsBossAncients/ArtWorks/Ancinets/donu.png";
	public override string? CustomMapIconOutlinePath => "res://StsBossAncients/ArtWorks/Ancinets/Outlines/donu.png";
	// 历史记录图标路径
	public override string? CustomRunHistoryIconPath => "res://StsBossAncients/ArtWorks/Ancinets/History/donu.png";
	public override string? CustomRunHistoryIconOutlinePath => "res://StsBossAncients/ArtWorks/Ancinets/History/donu.png";
	// 生成选项。每个选项有自己的池子。
	protected override OptionPools MakeOptionPools
	{
		get
		{
			RunState? state = RunManager.Instance.DebugOnlyGetState();
			var me = LocalContext.GetMe(state) ?? state?.Players.FirstOrDefault();

			int shiningWeight = me?.Character is Ironclad ? 999 : 0;
			int lazuliWeight = me?.Character is Silent ? 999 : 0;
			int coilWeight = me?.Character is Defect ? 999 : 0;
			int swordWeight = me?.Character is Regent ? 999 : 0;
			int friendWeight = me?.Character is Necrobinder ? 999 : 0;
			int waffeeWeight = (shiningWeight + lazuliWeight + coilWeight + swordWeight + friendWeight) == 0 ? 999 : 0;

			var thirdPool = MakePool(
				AncientOption<ShiningOintmentOctahedron>(weight: shiningWeight),
				AncientOption<LapisLazuli>(weight: lazuliWeight),
				AncientOption<CarryingCoil>(weight: coilWeight),
				AncientOption<CreatorSword>(weight: swordWeight),
				AncientOption<FriendshipProof>(weight: friendWeight),
				AncientOption<Waffle>(weight: waffeeWeight)
			);

			return new OptionPools(
				MakePool(
					AncientOption<Waffle>(),
					AncientOption<HeteroConcentric>()
				),
				MakePool(
					AncientOption<DonutDoll>(),
					AncientOption<EightBodyDoll>(),
					AncientOption<MechanicalDessert>()
				),
				thirdPool
			);
		}
	}
	
}

}
