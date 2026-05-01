using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace TheBrokenScript.Content.Tiles.Null;
public class Null : ModTile
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
		Main.tileMergeDirt[Type] = true;
		MinPick = 50;
		DustType = DustID.Ash;
		AddMapEntry(new Color(0, 0, 0));
	}
}
