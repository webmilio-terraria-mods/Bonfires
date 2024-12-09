using System.IO;
using Terraria.ModLoader;
using WebCom.Networking;

namespace Bonfires;

public class Bonfires : Mod
{
    public override void HandlePacket(BinaryReader reader, int whoAmI)
    {
        PacketLoader.This.HandlePacket(this, reader, whoAmI);
    }
}