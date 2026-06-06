using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using TheBrokenScript.Content.Subworlds.Nowhere;
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
		Player targetPlayer = activePlayers[Main.rand.Next(activePlayers.Count)];

		// Check biome.
		if (targetPlayer.InModBiome<VoidstoneBiome>())
		{
			return;
		}

		SpawnChestNearPlayer(targetPlayer);
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
		int playerTileX = (int)player.Center.X / 16;
		int playerTileY = (int)player.Center.Y / 16;
		int maxAttempts = 1000;
		int currentAttempts = 0;
		int x = 0;
		int y = 0;
		bool success = false;
		while (!success)
		{
			if (currentAttempts >= maxAttempts)
			{
				break;
			}
			currentAttempts++;
			x = Main.rand.Next(playerTileX - 10, playerTileX + 10);
			y = Main.rand.Next(playerTileY - 10, playerTileY + 10);
			int chestIndex = WorldGen.PlaceChest(x, y, type: 21, notNearOtherChests: false, style: 0);
			if (chestIndex != -1)
			{
				Chest chest = Main.chest[chestIndex];
				if (chest != null)
				{
					// Fill inventory first
					int itemSlot = 0;
					int slotsToFill = Main.rand.Next(1, 4);
					var itemTypePool = itemDropPool.OrderBy(itemType => Main.rand.Next()).Take(slotsToFill);
					foreach (int itemType in itemTypePool)
					{
						Item item = new Item();
						item.SetDefaults(itemType);
						item.stack = Main.rand.Next(1, 17);
						chest.item[itemSlot] = item;
						itemSlot++;
						if (itemSlot >= 40) break;
					}

					WorldGen.RangeFrame(chest.x - 1, chest.y - 1, chest.x + 3, chest.y + 3);

					int capturedIndex = chestIndex;
					int capturedX = chest.x;
					int capturedY = chest.y;
					int capturedSlots = itemSlot;

					// Defer sync to next tick so tile data is fully committed
					Main.QueueMainThreadAction(() =>
					{
						NetMessage.SendData(MessageID.ChestUpdates, -1, -1, null, 0, capturedX, capturedY, 0, capturedIndex);
						NetMessage.SendTileSquare(-1, capturedX - 1, capturedY - 1, 4, 4);
						for (int i = 0; i < capturedSlots; i++)
						{
							NetMessage.SendData(MessageID.SyncChestItem, -1, -1, null, capturedIndex, i);
						} 
					}); // Thanks GabeHasWon for the help. This took way too long.

					success = true;
					break;
				}
			}
		}
	}
}