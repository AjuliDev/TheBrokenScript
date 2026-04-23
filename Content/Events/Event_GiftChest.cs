using System;
using System.Linq;
using Terraria;
using Terraria.ID;
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
		ItemID.UnholyArrow,
		ItemID.HolyArrow,
		ItemID.Beenade,
		ItemID.PoopBlock,
		ItemID.SandBlock,
		ItemID.DirtBlock,
		ItemID.MudBlock,
		ItemID.StoneBlock,
		ItemID.AshWood,
		ItemID.ClayBlock
		];
	private void SpawnChestNearPlayer(Player player)
	{
		int playerTileX = (int)(player.position.X / 16);
		int playerTileY = (int)(player.position.Y / 16);
		for (int attempt = 0; attempt < 10; attempt++)
		{
			int x = playerTileX + Main.rand.Next(-20, 21);
			int y = playerTileY;
			while (y < Main.maxTilesY - 10 && !WorldGen.SolidTile(x, y))
			{
				y++;
			}
			y--;
			int chestIndex = WorldGen.PlaceChest(x, y, TileID.Containers, false, 1);
			if (chestIndex == -1)
			{
				continue;
			}
			Chest chest = Main.chest[chestIndex];
			chest.item[0].SetDefaults(itemDropPool[Main.rand.Next(0, itemDropPool.Length)]);
			chest.item[0].stack = Main.rand.Next(10, 21);
			if (Main.netMode == NetmodeID.Server)
			{
/*				foreach (RemoteClient client in Netplay.Clients.Where(c => c.IsActive))
				{
					client.TileSections[
						Netplay.GetSectionX(x),
						Netplay.GetSectionY(y)
						] = false;
				}*/
				NetMessage.SendTileSquare(-1, x - 1, y - 1, 4, 4);
				NetMessage.SendData(MessageID.SyncChestItem, -1, -1, null, chestIndex, 0);
			}

			break;
		}
	}
}