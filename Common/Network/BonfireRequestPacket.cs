using System;
using System.IO;
using Terraria.ModLoader;
using WebCom;

namespace Bonfires.Common.Network;

internal class BonfireRequestPacket : Packet
{
    protected override void PostReceive(BinaryReader reader, int fromWho)
    {
        if (!IsServer)
        {
            throw new NotSupportedException($"Only the server can receive packets of type {nameof(BonfireRequestPacket)}.");
        }

        ModContent.GetInstance<BonfireSystem>().Sync(fromWho);
    }
}
