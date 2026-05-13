using BaseLib.Abstracts;

namespace StsBossAncients.Scripts.Main
{
	public abstract class StsBossAncientsRelic : CustomRelicModel
{
	public override string PackedIconPath => $"res://StsBossAncients/ArtWorks/Relics/{Id.Entry}.png";
	protected override string PackedIconOutlinePath => $"res://StsBossAncients/ArtWorks/Relics/{Id.Entry}.png";
	protected override string BigIconPath => $"res://StsBossAncients/ArtWorks/Relics/{Id.Entry}.png";
}


}

