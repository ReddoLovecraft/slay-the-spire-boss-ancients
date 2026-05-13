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
using Timer = StsBossAncients.Scripts.Relics.Timer;

namespace StsBossAncients.Scripts.Ancients
{
public class TimeEater : ThirdFloorAncientModel
{
	// 选项按钮颜色
	public override Color ButtonColor => new Color(0.545f, 0.675f, 1.0f, 0.773f);
	// 对话框颜色
	public override Color DialogueColor => new Color(0.392f, 0.545f, 0.965f, 0.706f);
	// 自定义场景的路径
	public override string? CustomScenePath => "res://StsBossAncients/ArtWorks/Ancinets/Backgrounds/timeeater.tscn";
	// 自定义地图图标和轮廓的路径
	public override string? CustomMapIconPath => "res://StsBossAncients/ArtWorks/Ancinets/timeeater.png";
	public override string? CustomMapIconOutlinePath => "res://StsBossAncients/ArtWorks/Ancinets/Outlines/timeeater.png";
	// 历史记录图标路径
	public override string? CustomRunHistoryIconPath => "res://StsBossAncients/ArtWorks/Ancinets/History/timeeater.png";
	public override string? CustomRunHistoryIconOutlinePath => "res://StsBossAncients/ArtWorks/Ancinets/History/timeeater.png";
	// 生成选项。每个选项有自己的池子。
	protected override OptionPools MakeOptionPools
	{
		get
		{
			RunState? state = RunManager.Instance.DebugOnlyGetState();
			var me = LocalContext.GetMe(state) ?? state?.Players.FirstOrDefault();

			int photoWeight = me?.Character is Ironclad ? 999 : 0;
			int goldWeight = me?.Character is Silent ? 999 : 0;
			int ringWeight = me?.Character is Defect ? 999 : 0;
			int watchWeight = me?.Character is Regent ? 999 : 0;
			int bellWeight = me?.Character is Necrobinder ? 999 : 0;
			int instrumentWeight = (photoWeight + goldWeight + ringWeight + watchWeight + bellWeight) == 0 ? 999 : 0;

			var thirdPool = MakePool(
				AncientOption<OldPhoto>(weight: photoWeight),
				AncientOption<SilentGold>(weight: goldWeight),
				AncientOption<MobiusRing>(weight: ringWeight),
				AncientOption<AgelessWatch>(weight: watchWeight),
				AncientOption<DeadBell>(weight: bellWeight),
				AncientOption<ScribingInstrument>(weight: instrumentWeight)
			);

			return new OptionPools(
				MakePool(
					AncientOption<ScribingInstrument>(),
					AncientOption<AlarmBell>()
				),
				MakePool(
					AncientOption<Timer>(),
					AncientOption<TwistedPocketWatch>(),
					AncientOption<SandsOfTime>()
				),
				thirdPool
			);
		}
	}
	
}

}
