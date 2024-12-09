using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;

namespace Bonfires.Common.UI.CustomPanel;

public class TSPanel : UIPanel
{
    private const string TexturePath = "Common/UI/CustomPanel/";

    public Mod Mod { get; }

    private readonly Asset<Texture2D> _background, _borderH, _borderV, _corner;

    public TSPanel(Mod mod)
    {
        Mod = mod;

        BorderColor = Color.White;
        BackgroundColor = Color.White;

        _background = Mod.Assets.Request<Texture2D>(TexturePath + "PanelBackground");
        _borderH = Mod.Assets.Request<Texture2D>(TexturePath + "PanelBorderHorizontal");
        _borderV = Mod.Assets.Request<Texture2D>(TexturePath + "PanelBorderVertical");
        _corner = Mod.Assets.Request<Texture2D>(TexturePath + "PanelCorners");
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        var uiPos = GetDimensions().Position();

        //draw bg
        spriteBatch.End();

        // LMAO JK, we first need to use a proper sampler state
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.UIScaleMatrix);

        // now we draw BG tho
        spriteBatch.Draw(_background.Value, uiPos, new Rectangle(0, 0, (int)Width.Pixels, (int)Height.Pixels), BackgroundColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);

        // now we draw borders
        //top & bottom borders
        spriteBatch.Draw(_borderH.Value, uiPos + new Vector2(6, 0), new Rectangle(0, 0, (int)Width.Pixels - 12, 2), BorderColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
        spriteBatch.Draw(_borderH.Value, uiPos + new Vector2(6, Height.Pixels - 2), new Rectangle(0, 0, (int)Width.Pixels - 12, 2), BorderColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);

        // left & right borders
        spriteBatch.Draw(_borderV.Value, uiPos + new Vector2(0, 6), new Rectangle(0, 0, 2, (int)Height.Pixels - 12), BorderColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
        spriteBatch.Draw(_borderV.Value, uiPos + new Vector2(Width.Pixels - 2, 6), new Rectangle(0, 0, 2, (int)Height.Pixels - 12), BorderColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);

        //corners
        spriteBatch.Draw(_corner.Value, uiPos, new Rectangle(0, 0, 6, 6), BorderColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
        spriteBatch.Draw(_corner.Value, uiPos + new Vector2(Width.Pixels - 6, 0), new Rectangle(0, 0, 6, 6), BorderColor, 0f, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 1f);

        spriteBatch.Draw(_corner.Value, uiPos + new Vector2(0, Height.Pixels - 6), new Rectangle(0, 6, 6, 6), BorderColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
        spriteBatch.Draw(_corner.Value, uiPos + new Vector2(Width.Pixels - 6, Height.Pixels - 6), new Rectangle(0, 6, 6, 6), BorderColor, 0f, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 1f);
    }
}
