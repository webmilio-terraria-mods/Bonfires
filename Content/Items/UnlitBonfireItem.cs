using Bonfires.Content.Tiles.Bonfire;
using Terraria.ID;
using Terraria.ModLoader;

namespace Bonfires.Content.Items;

public sealed class UnlitBonfireItem : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 28;
        Item.height = 40;

        Item.rare = ItemRarityID.Orange;

        Item.useAnimation = 15;
        Item.useTime = 10;
        Item.useStyle = ItemUseStyleID.Swing;

        Item.consumable = true;
        Item.createTile = ModContent.TileType<BonfireTile>();
    }
}