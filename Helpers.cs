using Microsoft.Xna.Framework;
using Terraria;

namespace Bonfires;

internal static class Helpers
{
    public static Vector2 GetMultiTileTopLeft(int x, int y)
    {
        var tile = Main.tile[x, y];
        //return new(tile.TileFrameX, tile.TileFrameY);
        return new Vector2(x - tile.TileFrameX / 18 % 3, y - tile.TileFrameY / 18 % 3);
    }
}