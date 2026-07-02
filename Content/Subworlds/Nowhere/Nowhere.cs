using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using SubworldLibrary;
using System.Collections.Generic;
using System.Threading;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.WorldBuilding;
using thebrokenscript.Core;
namespace thebrokenscript.Content.Subworlds.Nowhere;
public class Nowhere : Subworld
{
	public static float UIRotation = 0f;
	public static float UIIslandScale = 0.1f;
	public override int Width => 2050;
	public override int Height => 650;
	public override bool ShouldSave => false;
	public override bool NoPlayerSaving => false;
	public override void OnEnter()
	{
		SubworldSystem.noReturn = false;
		SubworldSystem.hideUnderworld = true;
		UIRotation = 0f;
		UIIslandScale = 0f;
	}
	public override void OnLoad()
	{
		Main.spawnTileX = Width / 2;
		Main.spawnTileY = 250;
	}
	public override void OnExit()
	{
		UIRotation = 0f;
	}
	public override string Name => "Nowhere";
	public override bool ChangeAudio()
	{
		if (Main.gameMenu)
		{
			Main.newMusic = 0; // I learned this from WoTG. Thanks whoever made it.
			return true;
		}
		return false;
	}
	public override List<GenPass> Tasks => new List<GenPass>()
	{
		new FillTerrain(),
		new NoiseTerrain(),
		new CaveTerrain(),
		new MatterTerrain(),
	};
	public override void DrawMenu(GameTime gameTime)
	{
		var sb = Main.spriteBatch;
		if (!ModAssets.Textures.TryGetValue("NowhereMenuBG", out var nowhereMenuBG)) return;
		if (!ModAssets.Textures.TryGetValue("NowhereMenuIsland", out var NowhereMenuIsland)) return;
		if (!ModAssets.Textures.TryGetValue("NowhereMenuMeteors", out var NowhereMenuMeteors)) return;
		if (!ModAssets.Textures.TryGetValue("NowhereMenuClouds", out var NowhereMenuClouds)) return;
		UIRotation += 0.25f;
		if (UIRotation >= 360)
		{
			UIRotation = 0f;
		}
		UIIslandScale += 0.001f;
		Vector2 screenCenter = new Vector2(Main.screenWidth / 2f, Main.screenHeight / 2f);
		float bgScale = MathHelper.Max(
			Main.screenWidth / (float)nowhereMenuBG.Value.Width,
			Main.screenHeight / (float)nowhereMenuBG.Value.Height
			);
		Vector2 bgOrigin = new Vector2(nowhereMenuBG.Value.Width, nowhereMenuBG.Value.Height) / 2f;
		sb.Draw(nowhereMenuBG.Value, screenCenter, null, Color.White, 0f, bgOrigin, bgScale, SpriteEffects.None, 0f);
		sb.End();
		sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, effect: null, Matrix.Identity);

		// Island/Meteors/Clouds: natural size (scale = 1f), centered
		Vector2 islandOrigin = new Vector2(NowhereMenuIsland.Value.Width, NowhereMenuIsland.Value.Height) / 2f;
		Vector2 meteorOrigin = new Vector2(NowhereMenuMeteors.Value.Width, NowhereMenuMeteors.Value.Height) / 2f;
		Vector2 cloudsOrigin = new Vector2(NowhereMenuClouds.Value.Width, NowhereMenuClouds.Value.Height) / 2f;

		sb.Draw(NowhereMenuIsland.Value, screenCenter, null, Color.Black, rotation: MathHelper.ToRadians(UIRotation / 10f), islandOrigin, UIIslandScale, SpriteEffects.None, 0f);
		sb.Draw(NowhereMenuMeteors.Value, screenCenter, null, Color.White * 0.7f, rotation: MathHelper.ToRadians(UIRotation), meteorOrigin, 2f, SpriteEffects.None, 0f);
		sb.Draw(NowhereMenuClouds.Value, screenCenter, null, Color.White, rotation: MathHelper.ToRadians(-UIRotation), cloudsOrigin, 5f, SpriteEffects.None, 0f);
		sb.End();
		sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, effect: null, Matrix.Identity);

		sb.DrawString(FontAssets.DeathText.Value, Main.statusText,
			new Vector2(10f, Main.screenHeight - FontAssets.DeathText.Value.MeasureString(Main.statusText).Y - 10f),
			Color.White);

		//float baseScale = MathHelper.Max(Main.screenWidth / (float)nowhereMenuBG?.Value.Width,
		//	Main.screenHeight / (float)nowhereMenuBG?.Value.Height);
		//sb.Draw(nowhereMenuBG?.Value, new Vector2(0, 0), sourceRectangle: null, Color.White, rotation: 0f, origin: new Vector2(0,0), scale: new Vector2(baseScale * 1.1f, baseScale * 1f), SpriteEffects.None, layerDepth: 0f);

		//baseScale = MathHelper.Max(Main.screenWidth / (float)NowhereMenuIsland?.Value.Width,
		//	Main.screenHeight / (float)NowhereMenuIsland?.Value.Height);
		//sb.Draw(NowhereMenuIsland?.Value, new Vector2(0, 0), sourceRectangle: null, Color.White, rotation: 0f, origin: new Vector2(0, 0), scale: new Vector2(baseScale * 1.1f, baseScale * 1f), SpriteEffects.None, layerDepth: 0f);

		//baseScale = MathHelper.Max(Main.screenWidth / (float)NowhereMenuMeteors?.Value.Width,
		//	Main.screenHeight / (float)NowhereMenuMeteors?.Value.Height);
		//sb.Draw(NowhereMenuMeteors?.Value, new Vector2(0, 0), sourceRectangle: null, Color.White, rotation: 0f, origin: new Vector2(0, 0), scale: new Vector2(baseScale * 1.1f, baseScale * 1f), SpriteEffects.None, layerDepth: 0f);

		//baseScale = MathHelper.Max(Main.screenWidth / (float)NowhereMenuClouds?.Value.Width,
		//	Main.screenHeight / (float)NowhereMenuClouds?.Value.Height);
		//sb.Draw(NowhereMenuClouds?.Value, new Vector2(0, 0), sourceRectangle: null, Color.White, rotation: 0f, origin: new Vector2(0, 0), scale: new Vector2(baseScale * 1.1f, baseScale * 1f), SpriteEffects.None, layerDepth: 0f);

		//sb.DrawString(FontAssets.DeathText.Value, Main.statusText, new Vector2(Main.screenWidth, Main.screenHeight) / 2 - FontAssets.DeathText.Value.MeasureString(Main.statusText) / 2, Color.DarkRed);
	}
}
