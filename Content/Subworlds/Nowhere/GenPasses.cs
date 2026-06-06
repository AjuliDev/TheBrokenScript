using Microsoft.Xna.Framework;
using System;
using System.Threading;
using Terraria;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using TheBrokenScript.Content.Tiles.Corrupted;
using TheBrokenScript.Content.Tiles.Null;
using TheBrokenScript.Content.Tiles.RedObsidian;
using TheBrokenScript.Content.Tiles.Voidstone;
namespace TheBrokenScript.Content.Subworlds.Nowhere;
public class FillTerrain : GenPass
{
	public FillTerrain() : base("Terrain", 1) { }
	protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
	{
		progress.Message = "Approaching unknown territory...";
		Main.worldSurface = Main.maxTilesY - 400;
		Main.rockLayer = Main.maxTilesY;
		for (int x = 0; x < Main.maxTilesX; x++)
		{
			for (int y = 0; y < Main.maxTilesY; y++)
			{
				Tile tile = Main.tile[x, y];
				if (y < Main.worldSurface)
				{
					tile.HasTile = false;
				}
				else
				{
					tile.HasTile = true;
					tile.TileType = (ushort)ModContent.TileType<Voidstone>();
				}
			}
			progress.Value = (float)(x / Main.maxTilesX);
		}
		//Thread.Sleep(10000);
	}
}

public class NoiseTerrain : GenPass
{
	public NoiseTerrain() : base("Noise", 1) { }
	protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
	{
		progress.Message = "Detecting formations...";
		WorldGen._genRand = new Terraria.Utilities.UnifiedRandom((int)DateTime.Now.Ticks);
		Main.rand = new Terraria.Utilities.UnifiedRandom((int)DateTime.Now.Ticks + 1);
		FastNoiseLite noise = new FastNoiseLite();
		noise.SetSeed(WorldGen.genRand.Next());
		noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
		noise.SetFrequency(Main._rand.NextFloat(0.005f, 0.02f));
		noise.SetDomainWarpType(FastNoiseLite.DomainWarpType.OpenSimplex2Reduced);
		noise.SetDomainWarpAmp(0.5f);
		int maxHeight = 20;
		for (int x = 0; x < Main.maxTilesX; x++)
		{
			int actualSurface = 0;
			for (int y = 0; y < Main.maxTilesY; y++)
			{
				if (Main.tile[x, y].HasTile) { actualSurface = y; break; }
			}
			float noiseValue = noise.GetNoise(x, 0f);
			if (noiseValue > 0f)
			{
				int surfaceHeight = (int)(noiseValue * maxHeight);
				for (int y = actualSurface - surfaceHeight; y < actualSurface; y++)
				{
					if (y < 0 || y >= Main.maxTilesY) { continue; }
					Tile tile = Main.tile[x, y];
					tile.HasTile = true;
					tile.TileType = (ushort)ModContent.TileType<Voidstone>();
				}
			}
		}
		noise.SetFrequency(0.001f);
		for (int x = 0; x < Main.maxTilesX; x++)
		{
			int actualSurface = 0;
			for (int y = 0; y < Main.maxTilesY; y++)
			{
				if (Main.tile[x, y].HasTile) { actualSurface = y; break; }
			}
			float noiseValue = noise.GetNoise(x, 0f);
			if (noiseValue > 0f)
			{
				int surfaceHeight = (int)(noiseValue * (maxHeight + 20));
				for (int y = actualSurface - surfaceHeight; y < actualSurface; y++)
				{
					if (y < 0 || y >= Main.maxTilesY) { continue; }
					Tile tile = Main.tile[x, y];
					tile.HasTile = false;
					tile.TileType = (ushort)ModContent.TileType<Voidstone>();
				}
			}
		}
		noise.SetFrequency(0.05f);
		for (int x = 0; x < Main.maxTilesX; x++)
		{
			int actualSurface = 0;
			for (int y = 0; y < Main.maxTilesY; y++)
			{
				if (Main.tile[x, y].HasTile) { actualSurface = y; break; }
			}
			float noiseValue = noise.GetNoise(x, 0f);
			if (noiseValue > 0f)
			{
				int surfaceHeight = (int)(noiseValue * (maxHeight - 10));
				for (int y = actualSurface - surfaceHeight; y < actualSurface; y++)
				{
					if (y < 0 || y >= Main.maxTilesY) { continue; }
					Tile tile = Main.tile[x, y];
					tile.HasTile = true;
					tile.TileType = (ushort)ModContent.TileType<Voidstone>();
				}
			}
		}

		FastNoiseLite caveNoise = new FastNoiseLite();
		caveNoise.SetSeed(WorldGen.genRand.Next());
		caveNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
		caveNoise.SetFrequency(0.05f);
		for (int x = 0; x < Main.maxTilesX; x++)
		{
			for (int y = 0; y < Main.maxTilesY; y++)
			{
				if (!Main.tile[x, y].HasTile) continue;

				float noiseValue = caveNoise.GetNoise(x, y);

				if (noiseValue > 0.3f)
				{
					WorldGen.KillTile(x, y, false, false, true);
				}
			}
		}
	}
}
public class CaveTerrain : GenPass
{
	public CaveTerrain() : base("Caves", 1) { }
	protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
	{
		progress.Message = "Terrain seems unsafe...";
		WorldGen._genRand = new Terraria.Utilities.UnifiedRandom((int)DateTime.Now.Ticks);
		Main.rand = new Terraria.Utilities.UnifiedRandom((int)DateTime.Now.Ticks + 1);
		Main.worldSurface = Main.maxTilesY - 200;
		Main.rockLayer = Main.maxTilesY;
		int randomSpots = 100;
		for (int i = 0; i <= randomSpots; i++)
		{
			Vector2 currentSpot = new Vector2(Main.rand.Next(Main.maxTilesX), Main.rand.Next((int)Main.worldSurface, Main.maxTilesY));
			WorldGen.Cavinator((int)currentSpot.X, (int)currentSpot.Y, 80);
		}
	}
}
public class MatterTerrain : GenPass
{
	public MatterTerrain() : base("Matter", 1) { }
	protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
	{
		progress.Message = "Materialising matter...";
		ushort unstableDirtType = (ushort)ModContent.TileType<Voidstone>();
		ushort unstableMatterType = (ushort)ModContent.TileType<Corrupted>();
		ushort redObsidianType = (ushort)ModContent.TileType<RedObsidian>();
		for (int x = 0; x < Main.maxTilesX; x++)
		{
			for (int y = Main.maxTilesY - 1; y >= 1; y--) // bottom-up, skip y=0
			{
				Tile tile = Main.tile[x, y];
				if (tile.HasTile && tile.TileType == unstableDirtType)
				{
					Tile tileAbove = Main.tile[x, y - 1];
					if (!tileAbove.HasTile)
					{
						tileAbove.HasTile = true;
						tileAbove.TileType = unstableMatterType;
						tileAbove.Slope = tile.Slope;
						tileAbove.IsHalfBlock = tile.IsHalfBlock;
					}
				}
			}
		}

		for (int x = 0; x < Main.maxTilesX; x++)
		{
			for (int y = Main.maxTilesY - 1; y >= 1; y--) // bottom-up, skip y=0
			{
				Tile tile = Main.tile[x, y];
				if (tile.HasTile && tile.TileType == unstableDirtType)
				{
					Tile tileBelow = Main.tile[x, y + 1];
					Tile tileBelowNd = Main.tile[x, y + 2];
					if (!tileBelow.HasTile && !tileBelowNd.HasTile && Main.rand.NextBool(10))
					{
						tileBelow.HasTile = true;
						tileBelow.TileType = redObsidianType;
						tileBelowNd.HasTile = true;
						tileBelowNd.TileType = redObsidianType;
					}
				}
			}
		}
	}
}