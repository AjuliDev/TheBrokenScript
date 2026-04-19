using Terraria;
using Terraria.ModLoader;
namespace TheBrokenScript.Core;
public class ModWeather : ModSystem
{
	public override void PostUpdateTime()
	{
		if (Main.dedServ)
		{
			return;
		}
		ModState.WorldData worldData = ModState.GetWorldData();
	}
}