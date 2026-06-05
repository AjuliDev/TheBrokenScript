using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TheBrokenScript.Content.Tiles.Null;
using TheBrokenScript.Core;
namespace TheBrokenScript.Content.Events;

public class Event_Mangle : IModEvent
{
	public bool IsEnabled(ServerConfig config) => config.Event_Mangle;

	public void StartEvent()
	{
		var activePlayers = Main.player.Where(p => p.active).ToList();
		if (activePlayers.Count == 0)
		{
			return;
		}
		mangleTilesAroundPlayer(activePlayers[Main.rand.Next(activePlayers.Count)]);
	}
	private void mangleTilesAroundPlayer(Player player)
	{
		int playerTileX = (int)(player.position.X / 16);
		int playerTileY = (int)(player.position.Y / 16);
		int radius = 10; // Radius for the event
		var nearbyTileTypes = new HashSet<ushort>(); // HashSet of tiles
		for (int x = -radius; x <= radius; x++) // Scan for nearby tiles within the radius (unless I screwed something up lmao)
		{
			for (int y = -radius; y <= radius; y++)
			{
				int tileX = playerTileX + x;
				int tileY = playerTileY + y;
				if (tileX < 0 || tileY < 0 || tileX >= Main.maxTilesX || tileY >= Main.maxTilesY)
				{
					continue;
				}
				Tile tile = Main.tile[tileX, tileY];
				if (tile != null && tile.HasTile && Main.tileSolid[tile.TileType] && Main.tileMergeDirt[tile.TileType])
				{
					nearbyTileTypes.Add(tile.TileType); // Add new tile to hashset
				}
			}
		}
		nearbyTileTypes.Add((ushort)ModContent.TileType<Null>()); // To replace with corrupted tiles instead of null
		var tilePool = nearbyTileTypes.ToList();
		for (int x = -radius; x <= radius; x++)
		{
			for (int y = -radius; y <= radius; y++)
			{
				int tileX = playerTileX + x;
				int tileY = playerTileY + y;
				if (tileX < 0 || tileY < 0 || tileX >= Main.maxTilesX || tileY >= Main.maxTilesY)
				{
					continue;
				}
				Tile tile = Main.tile[tileX, tileY];
				if (tile == null || !tile.HasTile || TileID.Sets.IsAContainer[tile.TileType])
				{
					continue;
				}
				ushort newType = tilePool[Main.rand.Next(tilePool.Count)];
				tile.TileType = newType;
				WorldGen.SquareTileFrame(tileX, tileY, true);
			}
		}
		if (Main.netMode != NetmodeID.SinglePlayer)
		{
			NetMessage.SendTileSquare(-1, playerTileX, playerTileY, 24);
			// Broadcast the change to all players in server.
		}
		// Play Sound
		if (Main.dedServ)
		{
			ModPacket packet = ModContent.GetInstance<TheBrokenScript>().GetPacket();
			packet.Write((byte)ModPacketHandler.PacketType.RequestCaveSound);
			packet.Write(playerTileX);
			packet.Write(playerTileY);
			packet.Send(-1, -1);
		}
		else if (Main.netMode == NetmodeID.SinglePlayer)
		{
			ModSounds.PlayCaveNoise(playerTileX, playerTileY);
		}
	}
}