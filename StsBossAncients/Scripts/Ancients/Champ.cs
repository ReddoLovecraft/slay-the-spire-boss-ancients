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
public class Champ : SecondFloorAncientModel
{
	// 选项按钮颜色
	public override Color ButtonColor => new Color(0.449f, 0.652f, 0.973f, 0.8f);
	// 对话框颜色
	public override Color DialogueColor => new Color(0.333f, 0.632f, 0.904f, 0.796f);
	// 自定义场景的路径
	public override string? CustomScenePath => "res://StsBossAncients/ArtWorks/Ancinets/Backgrounds/champ.tscn";
	// 自定义地图图标和轮廓的路径
	public override string? CustomMapIconPath => "res://StsBossAncients/ArtWorks/Ancinets/champ.png";
	public override string? CustomMapIconOutlinePath => "res://StsBossAncients/ArtWorks/Ancinets/Outlines/champ.png";
	// 历史记录图标路径
	public override string? CustomRunHistoryIconPath => "res://StsBossAncients/ArtWorks/Ancinets/History/champ.png";
	public override string? CustomRunHistoryIconOutlinePath => "res://StsBossAncients/ArtWorks/Ancinets/History/champ.png";
	// 生成选项。每个选项有自己的池子。
	protected override OptionPools MakeOptionPools
	{
		get
		{
			RunState? state = RunManager.Instance.DebugOnlyGetState();
			var me = LocalContext.GetMe(state) ?? state?.Players.FirstOrDefault();

			int beltWeight = me?.Character is Ironclad ? 999 : 0;
			int venomWeight = me?.Character is Silent ? 999 : 0;
			int knightchessWeight = me?.Character is Defect ? 999 : 0;
			int guardWeight = me?.Character is Regent ? 999 : 0;
			int deviceWeight = me?.Character is Necrobinder ? 999 : 0;
			int armorWeight = (beltWeight + venomWeight + knightchessWeight + guardWeight + deviceWeight) == 0 ? 999 : 0;

			var thirdPool = MakePool(
				AncientOption<ChampionBelt>(weight: beltWeight),
				AncientOption<RoyalVenom>(weight: venomWeight),
				AncientOption<KnightChessPiece>(weight: knightchessWeight),
				AncientOption<RoyalGuard>(weight: guardWeight),
				AncientOption<ExecutionDevice>(weight: deviceWeight),
				AncientOption<KnightHeavyArmor>(weight: armorWeight)
			);

			return new OptionPools(
				MakePool(
					AncientOption<BrokenCrown>(),
					AncientOption<ConspiracyPlanBook>()
				),
				MakePool(
					AncientOption<GloryShield>(),
					AncientOption<HeroSword>(),
					AncientOption<KnightHeavyArmor>()
				),
				thirdPool
			);
		}
	}
	
}

}
