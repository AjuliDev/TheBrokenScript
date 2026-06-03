using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using TheBrokenScript.Core;

namespace TheBrokenScript.Common.EventHelpers;
public class KernelPanicModSystem : ModSystem
{
	public static bool IsActive = false;
	private bool HasSelectedImage = false;
	private Asset<Texture2D> SelectedTexture2D;
	private int DisplayTimeRegular = 5 * 60; // 5 seconds @ 60 ticks / 60 fps
	private int DisplayTimeFlash = 40;
	private int Timer = 0;
	private enum DisplayType
	{
		Regular,
		Flash,
		Inactive
	}
	private DisplayType Display = DisplayType.Inactive;
	private bool hasPlayedSound = false;
	public static void Enable() => IsActive = true;
	public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
	{
		int layerIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
		if (layerIndex != -1)
		{
			layers.Insert(layerIndex + 1, new LegacyGameInterfaceLayer("TBSA: Kernel Panic",
				delegate
				{
					if (IsActive)
					{
						// Get Texture
						if (!HasSelectedImage)
						{
							string[] keys = new string[]
							{
								"LinuxKernelPanic",
								"Windows11_BSOD",
								"Windows7_BSOD",
								"Windows8_BSOD",
								"WindowsXP_BSOD"
							};
							SelectedTexture2D = ModAssets.Textures[keys[Main.rand.Next(keys.Length)]];
							HasSelectedImage = true;
							keys = null;
						}
						// Get Display Type
						if (Display == DisplayType.Inactive)
						{
							Display = Main.rand.Next(0, 2) == 1 ? DisplayType.Regular : DisplayType.Flash;
						}
						// Play Sound
						if (!hasPlayedSound)
						{
							hasPlayedSound = true;
							if (Display == DisplayType.Regular)
							{
								ModAssets.Sounds["WeEncounteredAnError"].Value.Play(volume: 1.0f, pitch: 0.0f, 0.0f);
							}
							else
							{
								ModAssets.Sounds["NoiseBurst"].Value.Play(volume: 1.0f, pitch: 0.0f, 0.0f);
							}
						}
						// Check Against Cooldown and disable if necessary
						Main.spriteBatch.Draw(SelectedTexture2D?.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
						if ((Display == DisplayType.Regular && Timer >= DisplayTimeRegular) || (Display == DisplayType.Flash && Timer >= DisplayTimeFlash))
						{
							IsActive = false;
							HasSelectedImage = false;
							SelectedTexture2D = null;
							Timer = 0;
							Display = DisplayType.Inactive;
							hasPlayedSound = false;
						}
						// Advance Timer
						Timer++;
					}
					return true;
				},
				InterfaceScaleType.UI));
		}
	}
}
