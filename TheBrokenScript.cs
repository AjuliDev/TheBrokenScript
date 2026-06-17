using System.IO;
using Terraria.ModLoader;
using thebrokenscript.Core;
namespace thebrokenscript
{
	// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
	public class thebrokenscript : Mod
	{
		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			ModPacketHandler.Handle(reader, whoAmI);
		}
	}
}
