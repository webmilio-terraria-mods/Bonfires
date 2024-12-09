using Bonfires.Common.Configs;
using Bonfires.Content.Tiles.Bonfire;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Bonfires.Common;

internal class BonfireGenPass : GenPass
{
    public delegate void AppliedDelegate(BonfireGenPass pass, IList<Vector2> positions);
    public event AppliedDelegate Applied;

    private const double Ratio = 1d / (8400 * 2400);

    public BonfireGenPass() : base(nameof(Bonfires), 1f) { }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        var config = ModContent.GetInstance<BonfiresConfig>();

        progress.Message = "Planting Coiled Swords";
        double mapSize = Main.maxTilesX * Main.maxTilesY;

        // Inspired by https://github.com/tModLoader/tModLoader/blob/9455776e83860141dafb20fb8dee0e94fb560709/ExampleMod/ExampleWorld.cs#L180

        var bonfirePositions = new List<Vector2>();
        var bonfireTile = ModContent.TileType<BonfireTile>();

        var nbr = (int)(mapSize * config.LargeWorldBonfireCount * Ratio);

        while (bonfirePositions.Count < nbr)
        {
            var x = WorldGen.genRand.Next(0, Main.maxTilesX);
            var y = WorldGen.genRand.Next(0, Main.maxTilesY);

            if (Main.tile[x, y].TileType == bonfireTile)
            {
                continue;
            }

            var farEnough = true;
            var currentPosition = new Vector2(x, y);

            for (int j = 0; !farEnough && j < bonfirePositions.Count; j++)
            {
                if (Vector2.DistanceSquared(bonfirePositions[j], currentPosition) < (500 * 500))
                {
                    farEnough = true;
                }
            }

            if (!farEnough)
            {
                continue;
            }

            WorldGen.PlaceTile(x, y, bonfireTile);

            if (Main.tile[x, y].TileType == bonfireTile)
            {
                var topLeft = Helpers.GetMultiTileTopLeft(x, y);

                bonfirePositions.Add(topLeft);
                progress.Value = bonfirePositions.Count / (float)nbr;
            }
        }

        Applied?.Invoke(this, bonfirePositions);
    }
}