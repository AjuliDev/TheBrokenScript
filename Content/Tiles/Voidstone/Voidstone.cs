using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using thebrokenscript.Content.Tiles.Corrupted;
using thebrokenscript.Core;
namespace thebrokenscript.Content.Tiles.Voidstone;
public class Voidstone : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileBlockLight[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileFrameImportant[Type] = false;
		Main.tileCut[Type] = false;
		Main.tileAxe[Type] = false;
		Main.tileHammer[Type] = false;
		Main.tileMergeDirt[Type] = false;
		MinPick = 50;
		DustType = ModContent.DustType<CorruptedDust>();
		HitSound = new SoundStyle("thebrokenscript/Common/Sounds/VoidstoneHit");
		AddMapEntry(new Color(209, 209, 201));
	}
}
