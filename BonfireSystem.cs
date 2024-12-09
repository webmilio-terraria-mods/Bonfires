using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Bonfires.Common;
using Bonfires.Common.Network;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using Terraria.WorldBuilding;
using WebCom.Annotations;
using WebCom.Constants;
using WebCom.Networking;
using WebCom.Saving;
using WebCom.UI;

namespace Bonfires;

internal class BonfireSystem : ModSystem
{
    public delegate void BonfiresModifiedDelegate(BonfireSyncMode mode, IList<Vector2> positions, IList<Guid> ids);
    public event BonfiresModifiedDelegate BonfiresModified;

    [Save]
    private readonly Dictionary<Vector2, Guid> _bonfires = [];
    public IReadOnlyDictionary<Vector2, Guid> Bonfires { get; }

    public BonfireMenu Menu { get; private set; }
    public StdLayer Layer { get; private set; }

    public BonfireSystem()
    {
        Bonfires = new ReadOnlyDictionary<Vector2, Guid>(_bonfires);
    }

    public override void Load()
    {
        Menu = new(Mod);
        Layer = new(Menu);
    }

    public override void LoadWorldData(TagCompound tag)
    {
        _bonfires.Clear();

        Saver.This.Load(this, tag);
    }

    public override void SaveWorldData(TagCompound tag)
    {
        Saver.This.Save(this, tag);
    }

    public void Sync(int toWho)
    {
        var positions = new List<Vector2>(_bonfires.Count);
        var ids = new List<Guid>(_bonfires.Count);

        foreach (var (pos, id) in _bonfires)
        {
            positions.Add(pos);
            ids.Add(id);
        }

        Mod.PreparePacket(new BonfireSyncPacket
        {
            Mode = BonfireSyncMode.Add,
            Positions = [.. positions],
            Ids = [.. ids]
        })
            .Send(toWho);
    }

    public void Add(IList<Vector2> positions, IList<Guid> ids, bool propagate = true)
    {
        if (positions.Count != ids.Count)
        {
            throw new ArgumentException("The number of positions and ids must be the same.");
        }

        for (int i = 0; i < positions.Count; i++)
        {
            _bonfires.Add(positions[i], ids[i]);
        }

        BonfiresModified?.Invoke(BonfireSyncMode.Add, positions, ids);

        if (propagate)
        {
            Mod.PreparePacket(new BonfireSyncPacket
            {
                Mode = BonfireSyncMode.Add,
                Positions = [.. positions],
                Ids = [.. ids]
            })
                .Send();
        }
    }

    public bool TryGet(Vector2 position, out Guid id)
    {
        return _bonfires.TryGetValue(position, out id);
    }

    public int Remove(IList<Vector2> positions, bool propagate = true)
    {
        var removedPositions = new List<Vector2>(positions.Count);
        var removedIds = new List<Guid>(positions.Count);

        foreach (var position in positions)
        {
            if (!_bonfires.TryGetValue(position, out var id) || !_bonfires.Remove(position))
            {
                continue;
            }

            removedPositions.Add(position);
            removedIds.Add(id);
        }

        BonfiresModified?.Invoke(BonfireSyncMode.Remove, removedPositions, removedIds);

        if (removedPositions.Count > 0 && propagate)
        {
            Mod.PreparePacket(new BonfireSyncPacket
            {
                Mode = BonfireSyncMode.Remove,
                Positions = [.. removedPositions],
                Ids = [.. removedIds]
            })
                .Send();
        }

        return removedPositions.Count;
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        Layer.ModifyInterfaceLayers(layers, GameInterfaceLayerNames.Vanilla.ResourceBars);
    }

    public override void UpdateUI(GameTime gameTime)
    {
        Layer.Update(gameTime);
    }

    public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
    {
        var lifeCrystalsIndex = tasks.FindIndex(p => p.Name.Contains("Life Crystals"));

        var pass = new BonfireGenPass();
        pass.Applied += OnPass_Applied;

        tasks.Insert(lifeCrystalsIndex, pass);
    }

    private void OnPass_Applied(BonfireGenPass pass, IList<Vector2> positions)
    {
        pass.Applied -= OnPass_Applied;

        foreach (var position in positions)
        {
            _bonfires.Add(position, Guid.NewGuid());
        }
    }
}