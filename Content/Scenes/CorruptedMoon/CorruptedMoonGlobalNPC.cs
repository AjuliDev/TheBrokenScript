using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TheBrokenScript.Core;
namespace TheBrokenScript.Content.Scenes.CorruptedMoon;
public class CorruptedMoonGlobalNPC : GlobalNPC
{
	public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
	{
		var tsbTypes = new HashSet<int>
		{
			ModContent.NPCType<NPCs.Execute.Execute>(),
			ModContent.NPCType<NPCs.Observe.Observe>(),
			ModContent.NPCType<NPCs.SubAnomalyOne.SubAnomalyOne>(),
			ModContent.NPCType<NPCs.SiluetR2.SiluetR2>(),
			ModContent.NPCType<NPCs.Obliteration.Obliteration>(),
			ModContent.NPCType<NPCs.Follow.Follow>(),
			ModContent.NPCType<NPCs.Possessor.Possessor>(),
		};
		var worldData = ModState.GetWorldData();
		if (worldData.MoonData.MoonPhase != ModState.MoonPhase.Normal && !Main.IsItDay())
		{
			if (Main.invasionType == 0)
			{
				pool.Clear();
			}
			pool[ModContent.NPCType<NPCs.Execute.Execute>()] = 0.05f;
			pool[ModContent.NPCType<NPCs.Observe.Observe>()] = 0.025f;
			pool[ModContent.NPCType<NPCs.SubAnomalyOne.SubAnomalyOne>()] = 0.10f;
			pool[ModContent.NPCType<NPCs.SiluetR2.SiluetR2>()] = 0.10f;
			pool[ModContent.NPCType<NPCs.Obliteration.Obliteration>()] = 0.025f;
			pool[ModContent.NPCType<NPCs.Follow.Follow>()] = 0.05f;
			pool[ModContent.NPCType<NPCs.Possessor.Possessor>()] = 0.05f;

			if (Main.invasionType == 0)
			{
				foreach (var key in new List<int>(pool.Keys))
				{
					if (!tsbTypes.Contains(key))
						pool.Remove(key);
				}
			}
		}
		if (worldData.SunData.SunPhase != ModState.SunPhase.Normal && Main.IsItDay())
		{
			pool[ModContent.NPCType<NPCs.SiluetR2.SiluetR2>()] = 0.075f;
			pool[ModContent.NPCType<NPCs.SubAnomalyOne.SubAnomalyOne>()] = 0.075f;
			pool[ModContent.NPCType<NPCs.Observe.Observe>()] = 0.075f;
			pool[ModContent.NPCType<NPCs.Execute.Execute>()] = 0.075f;
			if (Main.invasionType == 0)
			{
				foreach (var key in new List<int>(pool.Keys))
				{
					if (!tsbTypes.Contains(key))
						pool.Remove(key);
				}
			}
		}
	}
	public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
	{
		var worldData = ModState.GetWorldData();
		if (worldData.MoonData.MoonPhase != ModState.MoonPhase.Normal && !Main.IsItDay()) // Checking light levels does not work that easily for this
		{
			spawnRate = 600; //600
			maxSpawns = 15; // 15
		}
		//if (worldData.MoonData.MoonPhase == ModState.MoonPhase.CorruptedRandom && !Main.IsItDay()) // TODO: Add some functionality around CorruptedRandom
		//{
		//	spawnRate = 0;
		//	maxSpawns = 0;
		//}
	}
}
