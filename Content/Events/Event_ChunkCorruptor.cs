using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using TheBrokenScript.Common.EventHelpers;
using TheBrokenScript.Core;

namespace TheBrokenScript.Content.Events;
public class Event_ChunkCorruptor : IModEvent
{
	public bool IsEnabled(ServerConfig config) => config.Event_Faint;
	public enum CorruptionType
	{
		Movement,
		Liquid
	}
	public void StartEvent()
	{
		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			return;
		}
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
			int targetPlayer = activePlayers[Main.rand.Next(activePlayers.Count)];
			Player playerInstance = Main.player[targetPlayer];
			MoveChunkUpward(Main.rand.Next((int)(playerInstance.Center.X / 16 - 50), (int)(playerInstance.Center.X / 16 + 50)), (int)(playerInstance.Center.Y / 16 + 20));
		}
		else
		{
			MoveChunkUpward(Main.rand.Next((int)(Main.LocalPlayer.Center.X / 16 - 50), (int)(Main.LocalPlayer.Center.X / 16 + 50)), (int)(Main.LocalPlayer.Center.Y / 16 + 20));
		}
	}
	private void MoveChunkUpward(int chunkX, int chunkY)
	{
		CorruptionType randomType = (CorruptionType)Main.rand.Next(Enum.GetValues(typeof(CorruptionType)).Length);
		if (randomType == CorruptionType.Movement)
		{
			// Init
			Point16 newOrigin = new Point16(Main.rand.Next(-40, 40) + chunkX, Main.rand.Next(-90, 90) + chunkY);
			int errorSizeSq = Main.rand.Next(5, 25);
			//Tile[,] tileSectorArray = new Tile[errorSizeSq, errorSizeSq];
			ushort[,] tileTypes = new ushort[errorSizeSq, errorSizeSq];
			ushort[,] wallTypes = new ushort[errorSizeSq, errorSizeSq];
			byte[,] liquidAmounts = new byte[errorSizeSq, errorSizeSq];
			byte[,] liquidTypes = new byte[errorSizeSq, errorSizeSq];
			short[,] tileFrameX = new short[errorSizeSq, errorSizeSq];
			short[,] tileFrameY = new short[errorSizeSq, errorSizeSq];
			bool[,] hasTile = new bool[errorSizeSq, errorSizeSq];
			bool[,] isHalfBlock = new bool[errorSizeSq, errorSizeSq];
			SlopeType[,] slopes = new SlopeType[errorSizeSq, errorSizeSq];
			byte[,] wallColors = new byte[errorSizeSq, errorSizeSq];
			bool[,] ignoredTileArray = new bool[errorSizeSq, errorSizeSq];
			int[,] tileStyleArray = new int[errorSizeSq, errorSizeSq];

			// Gather data
			for (int x = 0; x < errorSizeSq; x++)
			{
				for (int y = 0; y < errorSizeSq; y++)
				{
					if (!WorldGen.InWorld(x + chunkX, y + chunkY, 50))
					{
						continue;
					}
					Tile tile = Main.tile[chunkX + x, chunkY + y];
					//tileSectorArray[x, y] = new Tile();
					if (tile.HasTile && Main.tileFrameImportant[tile.TileType])
					{
						ignoredTileArray[x, y] = true;
						continue;
					}
					//ref var s = ref tileSectorArray[x, y];
					tileTypes[x, y] = tile.TileType;
					wallTypes[x, y] = tile.WallType;
					liquidAmounts[x, y] = tile.LiquidAmount;
					liquidTypes[x, y] = (byte)tile.LiquidType;
					tileFrameX[x, y] = tile.TileFrameX;
					tileFrameY[x, y] = tile.TileFrameY;
					hasTile[x, y] = tile.HasTile;
					isHalfBlock[x, y] = tile.IsHalfBlock;
					slopes[x, y] = tile.Slope;
					wallColors[x, y] = tile.WallColor;
					tileStyleArray[x, y] = TileObjectData.GetTileStyle(tile);
				}
			}

			// Remove sector
			for (int x = 0; x < errorSizeSq; x++)
			{
				for (int y = 0; y < errorSizeSq; y++)
				{
					if (!WorldGen.InWorld(x + chunkX, y + chunkY, 50))
					{
						continue;
					}
					if (!ignoredTileArray[x, y])
					{
						WorldGen.KillTile(x + chunkX, y + chunkY, fail: false, effectOnly: false, noItem: true);
					}
				}
			}

			// Write to new sector
			for (int x = 0; x < errorSizeSq; x++)
			{
				for (int y = 0; y < errorSizeSq; y++)
				{
					if (!WorldGen.InWorld(x + newOrigin.X, y + newOrigin.Y, 50))
					{
						continue;
					}
					if (!Main.tileFrameImportant[Main.tile[x + newOrigin.X, y + newOrigin.Y].TileType])
					{
						WorldGen.PlaceTile(x + newOrigin.X, y + newOrigin.Y, tileTypes[x, y], mute: true, forced: true, plr: -1, style: tileStyleArray[x, y]);
						Tile tile = Main.tile[x + newOrigin.X, y + newOrigin.Y];
						//ref var s = ref tileSectorArray[x, y];
						tile.WallType = wallTypes[x, y];
						tile.LiquidAmount = liquidAmounts[x, y];
						tile.LiquidType = liquidTypes[x, y];
						tile.TileFrameX = tileFrameX[x, y];
						tile.TileFrameY = tileFrameY[x, y];
						tile.IsHalfBlock = isHalfBlock[x, y];
						tile.Slope = slopes[x, y];
						tile.WallColor = wallColors[x, y];
					}
				}
			}

			// Frame old sector
			for (int x = chunkX - 2; x < chunkX + errorSizeSq + 2; x++)
			{
				for (int y = chunkY - 2; y < chunkY + errorSizeSq + 2; y++)
				{
					WorldGen.TileFrame(x, y);
					Framing.WallFrame(x, y);
				}
			}

			// Frame new sector
			for (int x = newOrigin.X - 2; x < newOrigin.X + errorSizeSq + 2; x++)
			{
				for (int y = newOrigin.Y - 2; y < newOrigin.Y + errorSizeSq + 2; y++)
				{
					WorldGen.TileFrame(x, y);
					Framing.WallFrame(x, y);
				}
			}

			// Sync tiles on server
			if (Main.dedServ)
			{
				NetMessage.SendTileSquare(-1, chunkX - 1, chunkY - 1, 24, 24);
				NetMessage.SendTileSquare(-1, newOrigin.X - 1, newOrigin.Y - 1, 24, 24);
			}
		}
		else if (randomType == CorruptionType.Liquid)
		{
			// Init
			Point16 newOrigin = new Point16(Main.rand.Next(-20, 20) + chunkX, Main.rand.Next(-50, 20) + chunkY);
			int errorSizeSq = Main.rand.Next(5, 15);

			// Write sector
			int liquidType = Main.rand.Next(0, 4); // Lava, water, honey, shimmer
			for (int x = 0; x < errorSizeSq; x++)
			{
				for (int y = 0; y < errorSizeSq; y++)
				{
					if (!WorldGen.InWorld(x + newOrigin.X, y + newOrigin.Y, 50))
					{
						continue;
					}
					if (!Main.tileFrameImportant[Main.tile[x + newOrigin.X, y + newOrigin.Y].TileType])
					{
						Tile tile = Main.tile[x + newOrigin.X, y + newOrigin.Y];
						tile.LiquidAmount = 255;
						tile.LiquidType = liquidType;
					}
				}
			}

			// Frame new sector
			for (int x = newOrigin.X - 2; x < newOrigin.X + errorSizeSq + 2; x++)
			{
				for (int y = newOrigin.Y - 2; y < newOrigin.Y + errorSizeSq + 2; y++)
				{
					WorldGen.TileFrame(x, y);
					Framing.WallFrame(x, y);
				}
			}

			// Sync tiles on server
			if (Main.dedServ)
			{
				NetMessage.SendTileSquare(-1, chunkX - 1, chunkY - 1, 16, 16);
				NetMessage.SendTileSquare(-1, newOrigin.X - 1, newOrigin.Y - 1, 16, 16);
			}
		}
	}
}
