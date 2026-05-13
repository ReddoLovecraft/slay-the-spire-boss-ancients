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
public class Awakened : ThirdFloorAncientModel
{
	// 选项按钮颜色
	public override Color ButtonColor => new Color(0.2f, 0.827f, 0.988f, 0.553f);
	// 对话框颜色
	public override Color DialogueColor => new Color(0.161f, 0.843f, 0.967f, 0.737f);
	// 自定义场景的路径
	public override string? CustomScenePath => "res://StsBossAncients/ArtWorks/Ancinets/Backgrounds/awakened.tscn";
	// 自定义地图图标和轮廓的路径
	public override string? CustomMapIconPath => "res://StsBossAncients/ArtWorks/Ancinets/awakened.png";
	public override string? CustomMapIconOutlinePath => "res://StsBossAncients/ArtWorks/Ancinets/Outlines/awakened.png";
	// 历史记录图标路径
	public override string? CustomRunHistoryIconPath => "res://StsBossAncients/ArtWorks/Ancinets/History/awakened.png";
	public override string? CustomRunHistoryIconOutlinePath => "res://StsBossAncients/ArtWorks/Ancinets/History/awakened.png";
	// 生成选项。每个选项有自己的池子。
	protected override OptionPools MakeOptionPools
	{
		get
		{
			RunState? state = RunManager.Instance.DebugOnlyGetState();
			var me = LocalContext.GetMe(state) ?? state?.Players.FirstOrDefault();

			int selfWeight = me?.Character is Ironclad ? 999 : 0;
			int coconWeight = me?.Character is Silent ? 999 : 0;
			int echoWeight = me?.Character is Defect ? 999 : 0;
			int starWeight = me?.Character is Regent ? 999 : 0;
			int soulWeight = me?.Character is Necrobinder ? 999 : 0;
			int RitualWeight = (selfWeight + coconWeight + echoWeight + starWeight + soulWeight) == 0 ? 999 : 0;

			var thirdPool = MakePool(
				AncientOption<SelfMolt>(weight: selfWeight),
				AncientOption<MysteriousCocoon>(weight: coconWeight),
				AncientOption<Phonograph>(weight: echoWeight),
				AncientOption<StarFruit>(weight: starWeight),
				AncientOption<DarkSoul>(weight: soulWeight),
				AncientOption<RitualDagger>(weight: RitualWeight)
			);

			return new OptionPools(
				MakePool(
					AncientOption<AwakeningTail>(),
					AncientOption<AwakeningPotion>()
				),
				MakePool(
					AncientOption<RitualDagger>(),
					AncientOption<BelieverSet>(),
					AncientOption<MassarethScripture>()
				),
				thirdPool
			);
		}
	}
	
}

}
