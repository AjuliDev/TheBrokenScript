using System.Linq;
using Terraria;
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
		int x = playerTileX + Main.rand.Next(-20, 20);
		int y = playerTileY;
		while (y < Main.maxTilesY - 10 && !WorldGen.SolidTile(x, y))
		{
			y++;
		}
		//WorldGen.OreRunner(x, y, 15, 15, (ushort)ModContent.TileType<Null>());
	}
}