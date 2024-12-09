using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria.ModLoader;
using WebCom;

namespace Bonfires.Common.Network;

internal class BonfireSyncPacket : Packet
{
    protected override void PostReceive(BinaryReader reader, int fromWho)
    {
        if (IsServer)
        {
            throw new NotSupportedException($"The server cannot receive packets of type {nameof(BonfireSyncPacket)}.");
        }

        if (Positions.Length != Ids.Length)
        {
            throw new ArgumentException("The number of positions and UUIDs must be the same.");
        }

        var system = ModContent.GetInstance<BonfireSystem>();

        switch (Mode)
        {
            case BonfireSyncMode.Add:
                system.Add(Positions, Ids, false);
                break;
            case BonfireSyncMode.Remove:
                system.Remove(Positions, false);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(Mode));
        }
    }

    public Vector2[] Positions { get; set; }
    public Guid[] Ids { get; set; }

    public BonfireSyncMode Mode { get; set; }
    public bool WorldSync { get; set; }
}
