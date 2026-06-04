using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using TheBrokenScript.Core;

namespace TheBrokenScript.Content.Events;

public class Event_RandomStructure : IModEvent
{
	public bool IsEnabled(ServerConfig config) => config.Event_RandomStructure;
	public enum SpawnLocation
	{
		Sky,
		Surface,
		Underground,
		Hell,
	}
	private static List<(SpawnLocation Location, string Name)> StructureList = new List<(SpawnLocation, string)>
	{
		(SpawnLocation.Hell, "RuinedPortal"),
		(SpawnLocation.Sky, "Lightway"),
		(SpawnLocation.Hell, "LeviathanSerpentBones"),
		(SpawnLocation.Underground, "Gullibility"),
		(SpawnLocation.Underground, "TheCriesOf"),
		(SpawnLocation.Surface, "ThornedVines"),
		(SpawnLocation.Sky, "HomeInfestation"),
		(SpawnLocation.Surface, "LihzahrdAncientRuins"),
		(SpawnLocation.Surface, "ScatteredRemains")
	};
	public void StartEvent()
	{
		// Get an active player, for reference
		Player targetPlayer;
		if (Main.dedServ)
		{
			List<int> activePlayers = [];
			for (int i = 0; i < Main.maxPlayers; i++)
			{
				if (Main.player[i].active)
				{
					activePlayers.Add(i);
				}
			}
			if (activePlayers.Count == 0)
			{
				return;
			}
			targetPlayer = Main.player[activePlayers[Main.rand.Next(activePlayers.Count)]];
		}
		else
		{
			targetPlayer = Main.LocalPlayer;
		}
		if (targetPlayer == null)
		{
			return;
		}

		// Find a location next to that player
		Point16 spawnRoot;
		SpawnLocation spawnEnvironment = GetSpawnLocation(targetPlayer);

		var validEntries = StructureList.Where(entry => entry.Location == spawnEnvironment).ToList();
		if (validEntries.Count == 0) return;
		var structureToSpawn = validEntries[Main.rand.Next(validEntries.Count)];

		spawnRoot = new Point16(
			Main.rand.Next((int)(targetPlayer.Center.X / 16f - 50), (int)(targetPlayer.Center.X / 16f + 50)),
			Main.rand.Next((int)(targetPlayer.Center.Y / 16f - 50), (int)(targetPlayer.Center.Y / 16f + 50))
			);

		if (spawnEnvironment == SpawnLocation.Surface)
		{
			int yLevel = (int)((targetPlayer.Center.Y / 16f) - 50f);
			for (int y = yLevel; y < targetPlayer.Bottom.Y + 15; y++)
			{
				if (Main.tile[spawnRoot.X, y].HasTile && Main.tileSolid[Main.tile[spawnRoot.X, y].TileType])
				{
					Point16 oldRoot = spawnRoot;
					spawnRoot = new Point16(oldRoot.X, y + 3);
					break;
				}
			}
		}
		
		if (!WorldGen.InWorld(spawnRoot.X, spawnRoot.Y, 50))
		{
			return;
		}

		string filePath = $"Common/Structures/Random/{structureToSpawn.Name}";
		var mod = ModContent.GetInstance<TheBrokenScript>();

		Point16 structureDimensions = StructureHelper.API.Generator.GetStructureDimensions(filePath, mod);

		StructureHelper.API.Generator.GenerateStructure(
			filePath,
			new Point16(spawnRoot.X, spawnRoot.Y - structureDimensions.Y),
			mod);

		// Frame structure and surroundings
		int structureX = spawnRoot.X;
		int structureY = spawnRoot.Y - structureDimensions.Y;
		for (int x = structureX - 5; x < structureX + structureDimensions.X + 5; x++)
		{
			for (int y = structureY - 5; y < structureY + structureDimensions.Y + 5; y++)
			{
				WorldGen.TileFrame(x, y);
				Framing.WallFrame(x, y);
			}
		}

		// Sync tiles on server
		if (Main.dedServ)
		{
			NetMessage.SendTileSquare(-1, structureX - 5, structureY - 5, structureDimensions.X + 10, structureDimensions.Y + 10);
		}
	}

	private static SpawnLocation GetSpawnLocation(Player player)
	{
		float tileY = player.position.Y / 16f;
		if (tileY < Main.worldSurface * 0.35f) return SpawnLocation.Sky;
		if (tileY < Main.worldSurface) return SpawnLocation.Surface;
		if (tileY < Main.maxTilesY - 200) return SpawnLocation.Underground;
		return SpawnLocation.Hell;
	}
}
