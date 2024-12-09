using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace Bonfires.Common.UI;

public class SilentButton : UIImageButton
{
    public SilentButton(Asset<Texture2D> texture) : base(texture)
    {
    }

    public override void MouseOver(UIMouseEvent evt)
    {
    }
}