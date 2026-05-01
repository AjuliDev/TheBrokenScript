using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TheBrokenScript.Core;
namespace TheBrokenScript.Content.Scenes.CorruptedMoon;
public class CorruptedMoonGlobalNPC : GlobalNPC
{
	public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
	{
		var worldData = ModState.GetWorldData();
		if (worldData.MoonData.MoonPhase != ModState.MoonPhase.Normal && !Main.IsItDay())
		{
			pool.Clear();
			pool[ModContent.NPCType<NPCs.Execute.Execute>()] = 0.05f;
			pool[ModContent.NPCType<NPCs.Observe.Observe>()] = 0.025f;
			pool[ModContent.NPCType<NPCs.SubAnomalyOne.SubAnomalyOne>()] = 0.10f;
			pool[ModContent.NPCType<NPCs.SiluetR2.SiluetR2>()] = 0.10f;
			pool[ModContent.NPCType<NPCs.Obliteration.Obliteration>()] = 0.025f;
			pool[ModContent.NPCType<NPCs.Follow.Follow>()] = 0.05f;
			pool[ModContent.NPCType<NPCs.Possessor.Possessor>()] = 0.05f;
		}
	}
	public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
	{
		var worldData = ModState.GetWorldData();
		if (worldData.MoonData.MoonPhase != ModState.MoonPhase.Normal && !Main.IsItDay())
		{
			spawnRate = 1200;
			maxSpawns = 10;
		}
	}
}
