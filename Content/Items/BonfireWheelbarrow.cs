using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Bonfires.Content.Items;

public sealed class BonfireWheelbarrow : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 32;
        Item.height = 32;

        Item.value = Item.sellPrice(gold: 50);
        Item.rare = ItemRarityID.Cyan;

        Item.useStyle = ItemUseStyleID.Thrust;
        Item.useTime = 15;
        Item.useAnimation = Item.useStyle;
    }
}