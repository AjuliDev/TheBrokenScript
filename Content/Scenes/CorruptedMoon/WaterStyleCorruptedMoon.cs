using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using thebrokenscript.Core;
namespace thebrokenscript.Content.Scenes.CorruptedMoon;
public class WaterStyleCorruptedMoon : ModWaterStyle
{
	public override int GetDropletGore() => Terraria.ID.GoreID.WaterDripBlood;
	public override int ChooseWaterfallStyle() => Terraria.ID.WaterStyleID.Bloodmoon;
	public override int GetSplashDust() => Terraria.ID.DustID.BloodWater;
	public override Color BiomeHairColor() => Color.Black;
	public override Asset<Texture2D> GetRainTexture() => ModAssets.Textures["CorruptedMoonRain"];
	public override void LightColorMultiplier(ref float r, ref float g, ref float b)
	{
		r = 0.1f;
		g = 0.1f;
		b = 0.1f;
	}
	public override byte GetRainVariant()
	{
		return (byte)Main.rand.Next(3);
	}
	public override string BlockTexture => "thebrokenscript/Content/Scenes/CorruptedMoon/CorruptedMoonWaterStyle_Block";
	public override string SlopeTexture => "thebrokenscript/Content/Scenes/CorruptedMoon/CorruptedMoonWaterStyle_Slope";
	public override string Texture => "thebrokenscript/Content/Scenes/CorruptedMoon/CorruptedMoonWaterStyle";
}
