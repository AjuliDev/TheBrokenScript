using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using TheBrokenScript.Content.Tiles.Corrupted;
using TheBrokenScript.Core;
namespace TheBrokenScript.Content.Tiles.RedObsidian;

public class RedObsidian : ModTile
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
		Main.tileLighted[Type] = true;
		Main.tileMergeDirt[Type] = false;
		MinPick = 100;
		DustType = DustID.Blood;
		HitSound = SoundID.Tink;
		AddMapEntry(new Color(175, 50, 50));
	}
	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		base.ModifyLight(i, j, ref r, ref g, ref b);
		r = 1;
		g = 0;
		b = 0;
	}
}
