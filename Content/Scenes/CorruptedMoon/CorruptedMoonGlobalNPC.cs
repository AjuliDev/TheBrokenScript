using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using TheBrokenScript.Core;
namespace TheBrokenScript.Content.Scenes.CorruptedMoon;
public class CorruptedMoonGlobalNPC : GlobalNPC
{
	public static HashSet<int> TBSTypes = new HashSet<int>
		{
			ModContent.NPCType<NPCs.Execute.Execute>(),
			ModContent.NPCType<NPCs.Observe.Observe>(),
			ModContent.NPCType<NPCs.SubAnomalyOne.SubAnomalyOne>(),
			ModContent.NPCType<NPCs.SiluetR2.SiluetR2>(),
			ModContent.NPCType<NPCs.Obliteration.Obliteration>(),
			ModContent.NPCType<NPCs.Follow.Follow>(),
			ModContent.NPCType<NPCs.Possessor.Possessor>(),
		};
	public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
	{
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
					if (!TBSTypes.Contains(key))
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
					if (!TBSTypes.Contains(key))
						pool.Remove(key);
				}
			}
		}
	}
	public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
	{
		var worldData = ModState.GetWorldData();
		var config = ServerConfig.Instance;
		if (worldData.MoonData.MoonPhase != ModState.MoonPhase.Normal && !Main.IsItDay()) // Checking light levels does not work that easily for this
		{
			spawnRate = 1600; //600
			if (config == null)
			{
				maxSpawns = 10;
			}
			else
			{
				maxSpawns = config.MaximumEntitiesAllowed;
			}
		}
		//if (worldData.MoonData.MoonPhase == ModState.MoonPhase.CorruptedRandom && !Main.IsItDay()) // TODO: Add some functionality around CorruptedRandom
		//{
		//	spawnRate = 0;
		//	maxSpawns = 0;
		//}
	}

	public override void OnSpawn(NPC npc, IEntitySource source)
	{
		base.OnSpawn(npc, source);

		if (TBSTypes.Contains(npc.type))
		{
			if (Main.dedServ)
			{
				ModPacket packet = ModContent.GetInstance<TheBrokenScript>().GetPacket();
				packet.Write((byte)ModPacketHandler.PacketType.RequestCaveSound);
				packet.Write((int)(npc.Center.X / 16f));
				packet.Write((int)(npc.Center.Y / 16f));
				packet.Send(-1, -1);
			}
			else if (Main.netMode == NetmodeID.SinglePlayer)
			{
				ModSounds.PlayCaveNoise((int)(npc.Center.X / 16f), (int)(npc.Center.Y / 16f));
			}
		}
	}
}
