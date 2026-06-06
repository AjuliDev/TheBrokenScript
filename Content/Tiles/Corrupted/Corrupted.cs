using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace TheBrokenScript.Content.Tiles.Corrupted;
public class Corrupted : ModTile
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
		DustType = DustID.Ash;
		AddMapEntry(new Color(0, 0, 0));
	}
	public override void EmitParticles(int i, int j, Tile tile, short tileFrameX, short tileFrameY, Color tileLight, bool visible)
	{
		//base.EmitParticles(i, j, tile, tileFrameX, tileFrameY, tileLight, visible);
		if (Main.rand.NextBool(5))
		{
			Dust.NewDust(new Vector2(i * 16f, j * 16f), 1, 1, ModContent.DustType<CorruptedDust>(), 0f, 0.5f, 0, Color.White, 25);
		}
	}
}
