using Terraria;
using Terraria.ModLoader;
using TheBrokenScript.Core;
namespace TheBrokenScript.Content.Scenes.CorruptedMoon;
public class CorruptedMoon : ModSceneEffect
{
	public override int Music => 0;
	public override string MapBackground => "TheBrokenScript/Content/Scenes/CorruptedMoon/CorruptedMoonMapBackground";
	public override ModWaterStyle WaterStyle => ModContent.GetInstance<WaterStyleCorruptedMoon>();
	public override SceneEffectPriority Priority => SceneEffectPriority.BossMedium;
	public override float GetWeight(Player player)
	{
		return 1.0f;
	}
	public override void SpecialVisuals(Player player, bool isActive)
	{
		if (isActive)
		{
			ModScenes.Activate("Posterize");
		}
		else
		{
			ModScenes.Deactivate("Posterize");
		}
	}
	public override bool IsSceneEffectActive(Player player)
	{
		var worldData = ModState.GetWorldData();
		if (worldData.MoonData.MoonPhase != ModState.MoonPhase.Normal && !Main.IsItDay())
		{
			return true;
		}
		else
		{
			return false;
		}
	}
}