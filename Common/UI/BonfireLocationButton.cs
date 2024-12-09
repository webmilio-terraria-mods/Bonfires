using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

namespace Bonfires.Common.UI;

public class BonfireLocationButton : SilentButton
{
    private Color _drawColor;

    private Tile BonfireTile { get; }
    private Vector2 BonfirePosition { get; }
    private string LocationName { get; }

    public BonfireLocationButton(string name, Vector2 pos, Tile tile) : base(TextureAssets.MagicPixel)
    {
        LocationName = name;
        BonfirePosition = pos;
        BonfireTile = tile;

        Height.Set(22, 0);
        Width.Set(240, 0);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        Vector2 mousePosition = new Vector2(Main.mouseX, Main.mouseY);

        if (ContainsPoint(mousePosition))
        {
            Main.hoverItemName = "Click to travel to bonfire's location";
            _drawColor = Color.Yellow;
        }
        else
        {
            _drawColor = Color.White;
        }

        Utils.DrawBorderString(spriteBatch, LocationName + " " + BonfirePosition, GetDimensions().Position(), _drawColor);

        //spriteBatch.Draw(Main.magicPixel, this.GetDimensions().Position() + new Vector2(0, 20), new Rectangle(0, 0, 270, 2), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
    }

    public override void LeftClick(UIMouseEvent evt)
    {
        if (!Main.LocalPlayer.GetModPlayer<BonfirePlayer>().TryTeleportToBonfire(BonfireTile))
        {
            Main.NewText("You couldn't teleport to the bonfire.");
        }

        ModContent.GetInstance<BonfireSystem>().Layer.Active = false;
    }
}
