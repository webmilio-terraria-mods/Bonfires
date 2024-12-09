using Bonfires.Common;
using Bonfires.Common.Configs;
using Bonfires.Content.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ModLoader;
using Terraria.ObjectData;
using WebCom.Tinq;

namespace Bonfires.Content.Tiles.Bonfire;

public class BonfireTile : ModTile
{
    private Asset<Texture2D> _texture;
    protected readonly Color mapEntryColor = Color.Orange;

    public override void SetStaticDefaults()
    {
        _texture = Mod.Assets.Request<Texture2D>("Content/Tiles/Bonfire/BonfireTile");

        Main.tileFrameImportant[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileLavaDeath[Type] = false;

        Main.tileSpelunker[Type] = true;
        Main.tileShine[Type] = 1200;
        Main.tileShine2[Type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
        TileObjectData.newTile.Origin = new Point16(1, 2);
        TileObjectData.newTile.CoordinateHeights = [16, 16, 16];
        TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);

        TileObjectData.addTile(Type);

        AnimationFrameHeight = 54;

        var tlName = CreateMapEntryName();
        AddMapEntry(mapEntryColor, tlName);
    }

    public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
    {
        return true;
    }

    public override bool RightClick(int x, int y)
    {
        var player = Main.LocalPlayer;

        if (player.HeldItem?.type == ModContent.ItemType<BonfireWheelbarrow>())
        {
            WorldGen.KillTile(x, y);
            // Item.NewItem(player.AsEntitySource(), x * 16, y * 16, 18, 48, ModContent.ItemType<UnlitBonfireItem>(), 1);

            return true;
        }

        var topLeft = Helpers.GetMultiTileTopLeft(x, y);
        x = (int)topLeft.X;
        y = (int)topLeft.Y;

        var modPlayer = BonfirePlayer.Get(player);
        var config = ModContent.GetInstance<BonfiresConfig>();

        if (config.SetSpawnOnMenu)
        {
            player.FindSpawn();

            if (player.SpawnX != x || player.SpawnY != y)
            {
                player.ChangeSpawn(x, y);
            }
        }

        var system = ModContent.GetInstance<BonfireSystem>();
        var tile = Main.tile[x, y];

        if (modPlayer.HasLitBonfire(tile))
        {
            system.Layer.Active = true;

            system.Menu.OnToggle();
            system.Menu.BonfirePosition = topLeft * 16;
        }
        else
        {
            modPlayer.LightBonfire(topLeft);

            Main.NewText("You have lit a bonfire!", Color.Orange);
        }

        return true;
    }

    public override void AnimateTile(ref int frame, ref int frameCounter)
    {
        if (++frameCounter >= 8)
        {
            frameCounter = 0;
            frame = ++frame % 7;
        }
    }

    public override bool PreDraw(int x, int y, SpriteBatch spriteBatch)
    {
        var tile = Main.tile[x, y];

        var tl = Helpers.GetMultiTileTopLeft(x, y);
        var tlTile = Main.tile[(int)tl.X, (int)tl.Y];

        var bonfireLit = BonfirePlayer.Get(Main.LocalPlayer).HasLitBonfire(tlTile);

        // Yeeted directly from Void Monolith in ExampleMod.
        var zero = new Vector2(Main.offScreenRange, Main.offScreenRange);

        if (Main.drawToScreen)
        {
            zero = Vector2.Zero;
        }

        var animate = 0;
        var position = new Vector2(x * 16 - (int)Main.screenPosition.X, y * 16 + 2 - (int)Main.screenPosition.Y) + zero;

        if (bonfireLit)
        {
            animate = (Main.tileFrame[Type] + 1) * 54;
            Lighting.AddLight(new Vector2(x * 16, y * 16), Color.Orange.ToVector3());
        }

        spriteBatch.Draw(_texture.Value, position, new Rectangle(tile.TileFrameX, tile.TileFrameY + animate, 16, 16), Lighting.GetColor(x, y), 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
        return false;
    }


    public override void KillTile(int x, int y, ref bool fail, ref bool effectOnly, ref bool noItem)
    {
        if (fail || effectOnly)
        {
            return;
        }

        Main.player.DoActive(p =>
            BonfirePlayer.Get(p).OnDestroyBonfire(Helpers.GetMultiTileTopLeft(x, y)));

        base.KillTile(x, y, ref fail, ref effectOnly, ref noItem);
    }

    public override bool CanKillTile(int x, int y, ref bool blockDamaged) => false;
    public override bool CanExplode(int x, int y) => false;
}