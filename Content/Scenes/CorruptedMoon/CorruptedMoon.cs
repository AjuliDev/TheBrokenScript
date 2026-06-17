using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using thebrokenscript.Content.Subworlds.Nowhere;
using thebrokenscript.Core;
namespace thebrokenscript.Content.Scenes.CorruptedMoon;
public class CorruptedMoon : ModSceneEffect
{
	public override int Music => SelectMusicTrack();
	public override string MapBackground => "thebrokenscript/Content/Scenes/CorruptedMoon/CorruptedMoonMapBackground";
	public override ModWaterStyle WaterStyle => ModContent.GetInstance<WaterStyleCorruptedMoon>();
	public override SceneEffectPriority Priority => SceneEffectPriority.Event;
	public override float GetWeight(Player player)
	{
		return 1.0f;
	}
	public override void SpecialVisuals(Player player, bool isActive)
	{
		var config = ClientConfig.Instance;
		if (config != null)
		{
			if (isActive && config.PosterizationShader)
			{
				ModScenes.Activate("Posterize");
			}
			else
			{
				ModScenes.Deactivate("Posterize");
			}
		}
		else
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
	}
	public override bool IsSceneEffectActive(Player player)
	{
		var worldData = ModState.GetWorldData();
		if (worldData.MoonData.MoonPhase != ModState.MoonPhase.Normal && !Main.IsItDay() || worldData.SunData.SunPhase != ModState.SunPhase.Normal && Main.IsItDay())
		{
			return true;
		}
		else if (player.InModBiome<VoidstoneBiome>())
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	public static readonly int SCPX4X = MusicLoader.GetMusicSlot("thebrokenscript/Common/Music/SCP-x4x");

	// Tuple with conditions and music ids
	private static readonly (Func<bool> condition, int track)[] MusicRules = 
		[
			(() => Main.invasionType == 1, SCPX4X), // goblin army
			(() => Main.invasionType == 2, SCPX4X), // snow legion
			(() => Main.invasionType == 3, SCPX4X), // pirate invasion
			(() => Main.invasionType == 4, SCPX4X), // martian madness
		];

	private int SelectMusicTrack()
	{
		foreach (var (condition, track) in MusicRules)
		{
			if (condition()) return track;
		}
		return 0; // no music, default
	}
}