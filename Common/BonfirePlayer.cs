using Bonfires.Common.Network;
using Bonfires.Content.Tiles.Bonfire;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using WebCom.Annotations;
using WebCom.Content;
using WebCom.Networking;
using WebCom.Saving;

namespace Bonfires.Common;

internal class BonfirePlayer : ModPlayer
{
    private const string
        WorldBonfiresTagName = "WorldBonfires",
        WorldBonfiresPrefix = "World_";

    public static BonfirePlayer Get(Player player) => player.GetModPlayer<BonfirePlayer>();

    [Save]
    private readonly Dictionary<Guid, List<Vector2>> _global = [];
    private readonly Dictionary<Tile, Vector2> _local = [];

    public IReadOnlyDictionary<Tile, Vector2> BonfirePositions { get; }

    public BonfirePlayer()
    {
        BonfirePositions = new ReadOnlyDictionary<Tile, Vector2>(_local);
    }

    public override void LoadData(TagCompound tag)
    {
        Saver.This.Load(this, tag);
    }

    public override void SaveData(TagCompound tag)
    {
        Saver.This.Save(this, tag);
    }

    public void LightBonfire(Vector2 bonfirePosition)
    {
        var tile = Main.tile[(int)bonfirePosition.X, (int)bonfirePosition.Y];

        if (!ModContent.GetInstance<BonfireSystem>().TryGet(bonfirePosition, out var id))
        {
            return;
        }

        _local.Add(tile, bonfirePosition);

        ModContent.GetInstance<WebComWorld>()
            .WaitForIdentifier(worldId => _global[worldId].Add(bonfirePosition));
    }

    public bool HasLitBonfire(Tile bonfire)
    {
        return _local.ContainsKey(bonfire);
    }

    public void OnDestroyBonfire(Vector2 bonfirePosition)
    {
        _local.Remove(Main.tile[(int)bonfirePosition.X, (int)bonfirePosition.Y]);
    }

    public bool TryTeleportToBonfire(Tile tile)
    {
        if (!_local.TryGetValue(tile, out var position))
        {
            return false; // The player doesn't know the Bonfire so they cannot teleport to it.
        }

        Player.Teleport(new Vector2(position.X * 16 + 16, position.Y * 16), 4);
        return true;
    }

    public override void OnEnterWorld()
    {
        var world = ModContent.GetInstance<WebComWorld>();
        world.WaitForIdentifier(OnReceivedWorldIdentifier);
    }

    private void OnReceivedWorldIdentifier(Guid id)
    {
        if (Main.netMode == NetmodeID.SinglePlayer)
        {
            OnEnterWorldLocal(id);
        }
        else
        {
            OnEnterWorldOnline();
        }
    }

    internal void OnEnterWorldOnline() => Mod.GetPacket<BonfireRequestPacket>().Send(); // And then the sync packet handles the rest.

    private void OnEnterWorldLocal(Guid worldId)
    {
        // We check if there are any Bonfires that have been lit for the player in this world.
        if (!_global.TryGetValue(worldId, out var bonfires))
        {
            _global.Add(worldId, []);
            return; // The are no registered Bonfires for this world, no need to run the rest of the code.
        }

        // We want to make sure no Bonfires are imported from a previous loaded world.
        _local.Clear();

        var tileType = ModContent.TileType<BonfireTile>();

        // We double-check that the Bonfires known to the player are still there.
        foreach (var position in bonfires)
        {
            var tile = Main.tile[(int)position.X, (int)position.Y];
            if (tile.TileType != tileType)
            {
                continue; // Is not a Bonfire anymore.
            }

            var currentTileTopLeft = Helpers.GetMultiTileTopLeft((int)position.X, (int)position.Y);
            if (currentTileTopLeft != position)
            {
                continue; // Bonfire was moved.
            }

            // Everything is fine with this Bonfire, so we save it.
            _local.Add(tile, position);
        }
    }
}