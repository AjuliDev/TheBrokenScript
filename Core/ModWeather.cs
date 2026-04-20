using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ModLoader;
namespace TheBrokenScript.Core;
public class ModWeather : ModSystem
{
	public override void Load()
	{
		On_Main.DrawSunAndMoon += On_Main_DrawSunAndMoon;
	}

	private void On_Main_DrawSunAndMoon(On_Main.orig_DrawSunAndMoon orig, Main self, Main.SceneArea sceneArea, Microsoft.Xna.Framework.Color moonColor, Microsoft.Xna.Framework.Color sunColor, float tempMushroomInfluence)
	{
		ModState.WorldData worldData = ModState.GetWorldData();
		if (worldData.MoonData.MoonPhase != ModState.MoonPhase.Normal)
		{
			int moonID = worldData.MoonData.MoonPhase == ModState.MoonPhase.CorruptedRandom ? worldData.MoonData.RandomCorruptedID : worldData.MoonData.CorruptedSequenceID;
			Asset<Texture2D> moonTexture = ModAssets.Textures[$"moon_{moonID}"];
			DrawCorruptedMoon(sceneArea, moonColor, moonTexture);
			if (Main.dayTime)
			{
				orig(self, sceneArea, moonColor, sunColor, tempMushroomInfluence);
			}
		}
		else
		{
			orig(self, sceneArea, moonColor, sunColor, tempMushroomInfluence);
		}
	}
	private void DrawCorruptedMoon(Main.SceneArea sceneArea, Microsoft.Xna.Framework.Color moonColor, Asset<Texture2D> moonTexture)
	{
		if (Main.dayTime) return;

		double num = Math.Pow(Math.Abs(Main.time / 32400.0 * 2.0 - 1.0), 2.0);
		int moonX = (int)(Main.time / 32400.0 * (double)(sceneArea.totalWidth + (float)(moonTexture.Value.Width * 2))) - moonTexture.Value.Width;
		int moonY = (int)((double)sceneArea.bgTopY + num * 250.0 + 180.0);
		float scale = (float)(1.2 - num * 0.4);
		float rotation = (float)(Main.time / 32400.0) * 2f - 7.3f;

		float alpha = 1f - Main.cloudAlpha * 1.5f * Main.atmo;
		if (alpha < 0f) alpha = 0f;
		moonColor *= alpha;

		Vector2 position = new Vector2(moonX, moonY + Main.moonModY);
		int w = moonTexture.Value.Width;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
		Main.spriteBatch.Draw(
			moonTexture.Value,
			position,
			new Rectangle(0, 0, w, w),
			moonColor,
			rotation,
			new Vector2(w / 2, w / 2),
			scale * 2f,
			SpriteEffects.None,
			0f
		);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
	}
	public override void Unload()
	{
		On_Main.DrawSunAndMoon -= On_Main_DrawSunAndMoon;
	}
}