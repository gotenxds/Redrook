using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using World;

namespace FlatBuffers
{
    public class Writer
    {
        private readonly Func<string, string> path = name => $"Assets/Resources/landscape/{name}.rrtd";
        private readonly TileReferenceFile tileReferenceFile;
        private readonly List<Offset<TileMap>> tileMaps;
        private readonly FlatBufferBuilder builder;
        private List<ushort> tilesPos;
        private ushort offset;

        public Writer()
        {
            tileReferenceFile = TileReferenceFile.Instance;
            tileReferenceFile.IsEditMode = true;

            tilesPos = new List<ushort>();
            builder = new FlatBufferBuilder(2048);
            offset = ushort.MaxValue;
            tileMaps = new List<Offset<TileMap>>();
        }


        public void WriteTile(Tile tile, Vector3Int tilePosition)
        {
            var tileNameIndex = tileReferenceFile.GetRefOf(tile.name);
            var tileIndex = Configs.CellPositionToIndex(tilePosition);
            
            if (offset == ushort.MaxValue)
            {
                offset = tileIndex;
            }

            tileIndex -= offset;

            if (tileIndex - tilesPos.Count >= 2)
            {
                tilesPos.AddRange(Enumerable.Repeat(ushort.MaxValue, tileIndex -1 - tilesPos.Count -1 ));
            }

            tilesPos.Add(tileNameIndex);
        }

        public void WriteTileMap()
        {
            tileMaps.Add(TileMap.CreateTileMap(builder, TileMap.CreateTilesVector(builder, tilesPos.ToArray()), offset));
            offset = ushort.MaxValue;
            tilesPos = new List<ushort>();
        }

        public void Save(string cellName)
        {
            var vectorOfTileMaps = builder.CreateVectorOfTables(tileMaps.ToArray());
            builder.Finish(Cell.CreateCell(builder, vectorOfTileMaps).Value);
            
            var bytes = builder.DataBuffer.ToSizedArray();
            File.WriteAllBytes(path(cellName), bytes);
            tileReferenceFile.Save();
        }
    }
}