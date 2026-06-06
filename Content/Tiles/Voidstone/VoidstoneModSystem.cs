using SubworldLibrary;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TheBrokenScript.Content.Subworlds.Nowhere;

namespace TheBrokenScript.Content.Tiles.Voidstone;

public class VoidstoneModSystem : ModSystem
{
	public int blockCount;
	public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
	{
		blockCount = tileCounts[ModContent.TileType<Voidstone>()];
	}

	public override void PostUpdateWorld()
	{
		if (SubworldSystem.IsActive<Nowhere>())
		{
			Main.dayTime = false;
			Main.time = 13000;
			if (Main.dedServ)
			{
				NetMessage.SendData(MessageID.WorldData);
			}
		}
	}
}
