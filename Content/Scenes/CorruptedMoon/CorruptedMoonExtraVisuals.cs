using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using TheBrokenScript.Core;

namespace TheBrokenScript.Content.Scenes.CorruptedMoon;
public class CorruptedMoonExtraVisuals : ModSystem
{
	private static float SmoothX, SmoothY;
	private static float currentHelperOpacity = 0f;
	public override void Load()
	{
		On_Main.DrawSurfaceBG += On_Main_DrawSurfaceBG;
		On_Main.DrawSunAndMoon += On_Main_DrawSunAndMoon;
	}

	private void On_Main_DrawSunAndMoon(On_Main.orig_DrawSunAndMoon orig, Main self, Main.SceneArea sceneArea, Color moonColor, Color sunColor, float tempMushroomInfluence)
	{
		var worldData = ModState.GetWorldData();
		if (worldData.SunData.SunPhase != ModState.SunPhase.Normal)
		{
			return;
		}
		if (worldData.MoonData.MoonPhase != ModState.MoonPhase.Normal)
		{
			return;
		}
		if (worldData.Timings.TotalNightsPassed >= 3)
		{
			return;
		}
		orig(self, sceneArea, moonColor, sunColor, tempMushroomInfluence);
	}

	private void On_Main_DrawSurfaceBG(On_Main.orig_DrawSurfaceBG orig, Main self) // Draw background is too choppy even with interpolation, avoid.
	{
		orig(self);
		var worldData = ModState.GetWorldData();
		var config = ClientConfig.Instance;
		if (config != null && config.ShowCorruptionVisionAssist == false)
		{
			return;
		}
		if (worldData.MoonData.MoonPhase == ModState.MoonPhase.Normal && !Main.IsItDay() || Main.IsItDay())
		{
			currentHelperOpacity = 0f;
			return;
		}
		if (!ModAssets.Textures.TryGetValue("corrupted_moon_bg", out var textureAsset))
		{
			return;
		}
		var texture = textureAsset?.Value;
		if (texture == null)
		{
			return;
		}
		var sb = Main.spriteBatch;
		Vector2 smoothScreenPosition = Main.Camera.UnscaledPosition;
		SmoothX = MathHelper.Lerp(SmoothX, Main.LocalPlayer.TopLeft.X, 0.15f);
		//SmoothY = MathHelper.Lerp(SmoothY, Main.LocalPlayer.TopLeft.Y + Main.rand.Next(0, 15), 0.15f);
		if (config == null)
		{
			SmoothY = MathHelper.Lerp(SmoothY, MathHelper.Clamp(
				Main.LocalPlayer.TopLeft.Y - 400f,
				(float)(Main.worldSurface * 16f * 0.6f), // adjust 0.6f for bringing the thing up or down
				(float)(Main.worldSurface * 16f)
				), 0.15f);
		}
		else
		{
			SmoothY = MathHelper.Lerp(SmoothY, MathHelper.Clamp(
				Main.LocalPlayer.TopLeft.Y - 400f,
				(float)(Main.worldSurface * 16f * config.CorruptionVisionAssistHeight), // adjust 0.6f for bringing the thing up or down
				(float)(Main.worldSurface * 16f)
				), 0.15f);
		}
		if (currentHelperOpacity < 1)
		{
			currentHelperOpacity += 0.001f;
		}
		currentHelperOpacity = MathHelper.Clamp(currentHelperOpacity, 0f, 1f);
		//Main.NewText(currentHelperOpacity);
		Vector2 worldAnchor = new Vector2(SmoothX, SmoothY);//(float)Main.worldSurface * 16f * 0.65f);
		Vector2 screenPosition = worldAnchor - smoothScreenPosition;
		//screenPosition *= Main.GameViewMatrix.Zoom;
		sb.End();
		sb.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, effect: null, Main.GameViewMatrix.TransformationMatrix);
		float baseScale = MathHelper.Max(Main.screenWidth / (float)texture.Width, Main.screenHeight / (float)texture.Height);
		sb.Draw(texture, screenPosition, sourceRectangle: null, Color.White * currentHelperOpacity, rotation: 0f, origin: texture.Size() / 2f, scale: new Vector2(baseScale * 1.1f, baseScale * 1f), SpriteEffects.None, layerDepth: 0f);
		sb.End();
		sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, effect: null, Main.GameViewMatrix.TransformationMatrix);
	}

	public override void Unload()
	{
		On_Main.DrawSurfaceBG -= On_Main_DrawSurfaceBG;
		On_Main.DrawSunAndMoon -= On_Main_DrawSunAndMoon;
	}
}
