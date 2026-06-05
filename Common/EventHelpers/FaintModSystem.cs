using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using TheBrokenScript.Core;

namespace TheBrokenScript.Common.EventHelpers;

public class FaintModSystem : ModSystem
{
	public static bool IsActive = false;
	private int DisplayTime = 8 * 60; // 8 seconds @ 60 ticks / 60 fps
	private int Timer = 0;
	private bool HasPlayedSound = false;
	public static void Enable()
	{
		IsActive = true;
		Main.LocalPlayer.mount.Dismount(Main.LocalPlayer);
		Main.LocalPlayer.AddBuff(BuffID.Blackout, 8 * 60, quiet: false);
		Main.LocalPlayer.AddBuff(BuffID.Stoned, 8 * 60, quiet: false);
		Main.LocalPlayer.AddBuff(BuffID.Darkness, 8 * 60, quiet: false);
	}
	public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
	{
		int layerIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: NPC / Sign Dialog"));
		if (layerIndex != -1)
		{
			layers.Insert(layerIndex + 1, new LegacyGameInterfaceLayer("TBSA: Faint",
				delegate
				{
					if (IsActive)
					{
						if (!HasPlayedSound)
						{
							HasPlayedSound = true;
							ModAssets.Sounds["Faint"].Value.Play(volume: 0.7f, pitch: 0.0f, 0.0f);
						}
						if (Timer >= DisplayTime)
						{
							IsActive = false;
							Timer = 0;
							HasPlayedSound = false;
						}
						Main.spriteBatch.Draw(Terraria.GameContent.TextureAssets.MagicPixel.Value, new Rectangle(0, 0, Main.screenWidth + 5, Main.screenHeight + 5), Color.Black);
						// Advance Timer
						Timer++;
					}
					return true;
				},
				InterfaceScaleType.UI));
		}
	}
}
