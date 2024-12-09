using Terraria;
using Terraria.ModLoader;

namespace Bonfires.Content.Tiles.Bonfire;

public sealed class GlobalBonfireTile : GlobalTile
{
    public override bool CanKillTile(int i, int j, int type, ref bool blockDamaged)
    {
        if (HasBonfireOnTop(i, j))
        {
            blockDamaged = true;
            return false;
        }

        return base.CanKillTile(i, j, type, ref blockDamaged);
    }

    public override bool CanExplode(int i, int j, int type)
    {
        if (HasBonfireOnTop(i, j))
            return false;

        return base.CanExplode(i, j, type);
    }


    private static bool HasBonfireOnTop(int i, int j) => TileLoader.GetTile(Main.tile[i, j - 1].TileType) is BonfireTile;
}