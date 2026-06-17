using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace thebrokenscript.Content.Tiles.RedObsidian;

public class RedObsidianItem : ModItem
{
	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.RedObsidian.RedObsidian>(), 0);
		Item.value = Terraria.Item.sellPrice(0, 0, 0, 25);
		Item.rare = ItemRarityID.Red;
	}
}
