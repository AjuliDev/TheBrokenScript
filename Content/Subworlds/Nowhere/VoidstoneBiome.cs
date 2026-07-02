using Terraria;
using Terraria.ModLoader;
using thebrokenscript.Content.Scenes.CorruptedMoon;
using thebrokenscript.Content.Tiles.Voidstone;
namespace thebrokenscript.Content.Subworlds.Nowhere;
public class VoidstoneBiome : ModBiome
{
	public override ModWaterStyle WaterStyle => ModContent.GetInstance<WaterStyleCorruptedMoon>();
	public override int Music => 0;
	public override bool IsBiomeActive(Player player)
	{
		bool enoughBlocks = ModContent.GetInstance<VoidstoneModSystem>().blockCount >= 40;
		return enoughBlocks;
	}
	public override SceneEffectPriority Priority => SceneEffectPriority.Environment;
}
