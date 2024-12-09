using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace Bonfires.Common.Configs;

internal class BonfiresConfig : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ServerSide;

    [DefaultValue(true)]
    public bool SetSpawnOnMenu { get; set; }

    [DefaultValue(50)]
    [Range(0, 200)]
    public int LargeWorldBonfireCount { get; set; }
}
