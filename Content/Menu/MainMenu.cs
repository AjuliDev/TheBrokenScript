using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using TheBrokenScript.Core;

namespace TheBrokenScript.Content.Menu;
public class MainMenu : ModMenu
{
	private bool JoinDiscordClicked = false;
	public override int Music => MusicLoader.GetMusicSlot(Mod, "Common/Music/SCP-x4x");
	public override string DisplayName => "The Broken Script: Aftermath - 0.23.0 - Overhaul Of The Basics";
	public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor)
	{
		var sb = spriteBatch;
		//sb.End();
		//sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);
		sb.Draw(ModAssets.Textures["Wallpaper"].Value, new Rectangle(-100, -100, Main.screenWidth + 200, Main.screenHeight + 200), Color.White);
		sb.Draw(ModAssets.Textures["TerrariaLogo"].Value, new Vector2(Main.screenWidth / 2, 100f), null, Color.White, logoRotation, ModAssets.Textures["TerrariaLogo"].Value.Size() * 0.5f, logoScale, SpriteEffects.None, 0f);
		sb.Draw(ModAssets.Textures["WorkInProgress"].Value, new Vector2(220f, Main.screenHeight - 100f), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
		sb.Draw(ModAssets.Textures["JoinTheDiscord"].Value, new Vector2(220f, Main.screenHeight - 50f), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

		Vector2 TexturePosition = new Vector2(220f, Main.screenHeight - 50f);
		Rectangle TextureRectangle = new Rectangle((int)TexturePosition.X, (int)TexturePosition.Y, (int)ModAssets.Textures["JoinTheDiscord"].Value.Width, (int)ModAssets.Textures["JoinTheDiscord"].Value.Height);
		bool TextureContainsMouse = TextureRectangle.Contains(Main.mouseX, Main.mouseY);
		if (TextureContainsMouse)
		{
			Main.instance.MouseText("Join The Broken Script: Aftermath's Discord Server! Click!");
			if (Main.mouseLeft && !JoinDiscordClicked)
			{
				JoinDiscordClicked = true;
				SoundEngine.PlaySound(SoundID.AchievementComplete);
				try
				{
					Process.Start(new ProcessStartInfo
					{
						FileName = "https://discord.gg/5VmjMzVHPE",
						UseShellExecute = true
					});
				}
				catch
				{

				}
				if (!Main.mouseLeft)
				{
					JoinDiscordClicked = false;
				}
			}
		}

		return false;
	}
}
