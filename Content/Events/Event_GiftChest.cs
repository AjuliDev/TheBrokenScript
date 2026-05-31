using System;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using TheBrokenScript.Content.Tiles.Null;
using TheBrokenScript.Core;
namespace TheBrokenScript.Content.Events;
public class Event_GiftChest : IModEvent
{
	public bool IsEnabled(ServerConfig config) => config.Event_GiftChest;
	public void StartEvent()
	{
		var activePlayers = Main.player.Where(p => p.active).ToList();
		if (activePlayers.Count == 0)
		{
			return;
		}
		SpawnChestNearPlayer(activePlayers[Main.rand.Next(activePlayers.Count)]);
	}
	private int[] itemDropPool = [
		ItemID.GoldBar,
		ItemID.LeadBar,
		ItemID.IronBar,
		ItemID.CopperBar,
		ItemID.MeteoriteBar,
		ItemID.TungstenBar,
		ItemID.GoldCoin,
		];
	private void SpawnChestNearPlayer(Player player)
	{
		int playerTileX = (int)(player.position.X / 16);
		int playerTileY = (int)(player.position.Y / 16);
		for (int attempt = 0; attempt < 10; attempt++)
		{
			int x = playerTileX + Main.rand.Next(-20, 21);
			int y = playerTileY - 4;
			while (y < Main.maxTilesY - 10 && !Main.tile[x, y].HasTile)
			{
				y++;
			}
			y--; //Retract from solid tile
			bool hasPlacedChest = WorldGen.PlaceObject(x, y, TileID.Containers, true, 0);
			if (!hasPlacedChest)
			{
				continue;
			}

			Point16 topLeft = TileObjectData.TopLeft(x, y);
			int chestID = Chest.CreateChest(topLeft.X, topLeft.Y);
			if (chestID == -1)
			{
				continue;
			}

			Chest chestTile = Main.chest[chestID];
			chestTile.item[0].SetDefaults(itemDropPool[Main.rand.Next(0, itemDropPool.Length)]);
			chestTile.item[0].stack = Main.rand.Next(1, 16);

			WorldGen.PlaceTile(topLeft.X, topLeft.Y + 2, ModContent.TileType<Null>(), forced: true);
			WorldGen.PlaceTile(topLeft.X + 1, topLeft.Y + 2, ModContent.TileType<Null>(), forced: true);

			if (Main.netMode != NetmodeID.SinglePlayer)
			{
				NetMessage.SendData(MessageID.ChestUpdates, -1, -1, null, chestID, topLeft.X, topLeft.Y);
				NetMessage.SendData(MessageID.SyncChestItem, -1, -1, null, chestID, 0);
				NetMessage.SendTileSquare(-1, topLeft.X, topLeft.Y, 2, 4);
			}
			break;
		}
	}
}