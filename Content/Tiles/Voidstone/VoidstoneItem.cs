using Terraria.ID;
using Terraria.ModLoader;

namespace thebrokenscript.Content.Tiles.Voidstone
{
	public class VoidstoneItem : ModItem
	{
		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Voidstone.Voidstone>(), 0);
			Item.value = Terraria.Item.sellPrice(0, 0, 0, 1);
			Item.rare = ItemRarityID.Gray;
		}
	}
}
