using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace thebrokenscript.Common;
public class SubworldCounter : ModSystem
{
	public static int NowhereEntryCooldown = 0;
	public override void PostUpdateTime()
	{
		if (Main.dedServ)
		{
			if (NowhereEntryCooldown > 0)
			{
				Task.Delay(1000 * NowhereEntryCooldown).ContinueWith(task => NowhereEntryCooldown = 0);
			}
		}
	}
}
