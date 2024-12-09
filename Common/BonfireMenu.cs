using Bonfires.Common.UI;
using Bonfires.Common.UI.CustomPanel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;

namespace Bonfires.Common;

public sealed class BonfireMenu : UIState
{
    private readonly Mod _mod;
    private UIScrollbar _scrollBar;
    private UIGrid _locations;

    private TSPanel _mainPanel;

    public Vector2 BonfirePosition { get; set; }

    public BonfireMenu(Mod mod)
    {
        _mod = mod;
    }

    public override void OnInitialize()
    {
        _mainPanel = new TSPanel(_mod);

        _mainPanel.Width.Set(400, 0);
        _mainPanel.Height.Set(17 * 26, 0);

        _mainPanel.VAlign = 0.5f;
        _mainPanel.HAlign = 0.5f;

        _mainPanel.SetPadding(15);
        Append(_mainPanel);

        float mainPanelElementsHeight = _mainPanel.Height.Pixels - (_mainPanel.PaddingTop + _mainPanel.PaddingBottom);

        _scrollBar = new UIScrollbar();
        _scrollBar.Width.Set(100, 0f);
        _scrollBar.Height.Set(0f, 1f);
        _scrollBar.SetView(132, 600);

        _mainPanel.Append(_scrollBar);

        float minimumSpacingToScrollBar = _scrollBar.Width.Pixels + _mainPanel.PaddingLeft;

        var bgPanel = new UIPanel();

        float widthLeft = _mainPanel.Width.Pixels - minimumSpacingToScrollBar;

        bgPanel.Width.Set(0, (widthLeft - _mainPanel.PaddingRight * 1.5f) / widthLeft);
        bgPanel.Height.Set(mainPanelElementsHeight, 0);
        bgPanel.Left.Set(_mainPanel.PaddingLeft * 2, 0);
        bgPanel.BackgroundColor = Color.Transparent;

        _mainPanel.Append(bgPanel);

        _locations = new UIGrid();
        _locations.Width.Set(_mainPanel.Width.Pixels - minimumSpacingToScrollBar, 0);
        _locations.Height.Set(mainPanelElementsHeight, 0);

        bgPanel.Append(_locations);
        _locations.SetScrollbar(_scrollBar);
    }

    public void OnToggle()
    {
        _locations.Clear();
        _scrollBar.ViewPosition = 0f;

        var player = BonfirePlayer.Get(Main.LocalPlayer);

        foreach (var kvp in player.BonfirePositions)
        {
            _locations.Add(new BonfireLocationButton("Bizzare Location", kvp.Value, kvp.Key));
        }
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        if (_mainPanel.ContainsPoint(Main.MouseScreen))
        {
            Main.LocalPlayer.mouseInterface = true;
        }

        Main.LocalPlayer.frozen = true;

        Recalculate();
        RecalculateChildren();
    }
}