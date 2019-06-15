using System;
using System.Collections.Generic;
using FlatBuffers;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CellLoading
{
    public sealed class TileCache
    {
        private static readonly string Path = "TileMaps/Tiles";
        private static readonly Lazy<TileCache> Reference = new Lazy<TileCache>(() => new TileCache());
        private readonly Dictionary<int, WeakReference<Tile>> cache;
        private readonly TileReferenceFile tileReferenceFile;
        private TileCache()
        {
            tileReferenceFile = TileReferenceFile.Instance;
            cache = new Dictionary<int, WeakReference<Tile>>();
        }

        public static TileCache Instance => Reference.Value;

        public Tile GetTile(ushort index)
        {
            Tile tile;
            WeakReference<Tile> tileRef;
            
            if (cache.TryGetValue(index, out tileRef) && tileRef.TryGetTarget(out tile))
            {
                return tile;
            }

            var tileName = tileReferenceFile.GetTileName(index);
            tile = Resources.Load<Tile>($"{Path}/{tileName}");

            cache.Add(index, new WeakReference<Tile>(tile));

            return tile;
        }
    }
}