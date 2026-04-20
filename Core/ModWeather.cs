using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
namespace TheBrokenScript.Core;
public class ModWeather : ModSystem
{
	private Asset<Texture2D>[] originalMoon;
	public override void Load()
	{
		On_Main.DrawSunAndMoon += On_Main_DrawSunAndMoon;
		originalMoon = TextureAssets.Moon.ToArray();
	}

	private void On_Main_DrawSunAndMoon(On_Main.orig_DrawSunAndMoon orig, Main self, Main.SceneArea sceneArea, Microsoft.Xna.Framework.Color moonColor, Microsoft.Xna.Framework.Color sunColor, float tempMushroomInfluence)
	{
		ModState.WorldData worldData = ModState.GetWorldData();
		if (worldData.MoonData.MoonPhase != ModState.MoonPhase.Normal)
		{
			int moonID = worldData.MoonData.MoonPhase == ModState.MoonPhase.CorruptedRandom ? worldData.MoonData.RandomCorruptedID : worldData.MoonData.CorruptedSequenceID;
			Asset<Texture2D> moonTexture = ModAssets.Textures[$"moon{moonID}"];
			Array.Fill(TextureAssets.Moon, moonTexture);
			orig(self, sceneArea, moonColor, sunColor, tempMushroomInfluence);
			for (int i = 0; i < originalMoon.Length; i++)
			{
				TextureAssets.Moon[i] = originalMoon[i];
			}
		}
		else
		{
			orig(self, sceneArea, moonColor, sunColor, tempMushroomInfluence);
		}
	}

	public override void Unload()
	{
		On_Main.DrawSunAndMoon -= On_Main_DrawSunAndMoon;
		originalMoon = null;
	}
}