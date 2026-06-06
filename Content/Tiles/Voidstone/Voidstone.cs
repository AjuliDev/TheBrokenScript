using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using TheBrokenScript.Content.Tiles.Corrupted;
using TheBrokenScript.Core;
namespace TheBrokenScript.Content.Tiles.Voidstone;
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
		HitSound = new SoundStyle("TheBrokenScript/Common/Sounds/VoidstoneHit");
		AddMapEntry(new Color(209, 209, 201));
	}
}
