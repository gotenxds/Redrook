using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utils;

namespace World
{
    public class CellWrapper
    {
        private bool isDirty;
        private string name;
        private Vector3Int cellPosition;
        private Cell cell;
        private Dictionary<ushort, List<ushort>> dirtyPositions;

        public CellWrapper(ref Cell cell)
        {
            dirtyPositions = new Dictionary<ushort, List<ushort>>();
            this.cell = cell;
        }

        public bool IsDirty
        {
            get => isDirty;
            private set => isDirty = value;
        }

        public string Name
        {
            get => name;
            set
            {
                name = value;
                cellPosition = CellUtils.GetWorldCellPosition(name);
            }
        }

        public string Path { get; set; }

        

        public ushort GetTileRefIndex(ushort mapIndex, Vector3Int worldPosition)
        {
            var tileMap = cell.TileMaps(mapIndex).Value;

            var index = WorldToLocal(worldPosition, tileMap.Offset);

            return InRange(index, tileMap) ? tileMap.Tiles(index) : ushort.MaxValue;
        }

        public IEnumerable<Vector3Int> GetBounds(ushort tileMapIndex)
        {
            var allPositionsWithin = new BoundsInt(cellPosition, new Vector3Int(
                cellPosition.x + Configs.CellSideSize,
                cellPosition.y + Configs.CellSideSize, 1
            )).allPositionsWithin;

            var positions = new List<Vector3Int>();
            
            foreach (var vector3Int in allPositionsWithin)
            {
                positions.Add(vector3Int);
            }

            return positions;
        }
        
        public IEnumerable<ushort> GetAllTilesLocations(ushort tileMapIndex)
        {
            var positions = new List<ushort>(dirtyPositions[tileMapIndex]);

            var tileMap = cell.TileMaps(tileMapIndex).Value;
            
            for (int i = 0; i < tileMap.TilesLength; i++)
            {
                positions.Add((ushort) (tileMap.Tiles(i) - tileMap.Offset));
            }

            return positions;
        }

        public void MarkDirty(ushort tileMapIndex, Vector3Int position, ushort tileRef)
        {
            List<ushort> dirty;
            if (!dirtyPositions.TryGetValue(tileMapIndex, out dirty))
            {
                dirty = new List<ushort>();
                dirtyPositions.Add(tileMapIndex, dirty);
            }
            
            dirty.Add(WorldToLocal(position, 0));
            isDirty = true;
        }

        private ushort WorldToLocal(Vector3Int worldPosition, ushort offset)
        {
            var localPosition = worldPosition - cellPosition;
            var index = localPosition.x + localPosition.y * Configs.CellSideSize - offset;
            
            return (ushort) index;
        }

        private static bool InRange(int index, TileMap tileMap)
        {
            return index >= 0 && index < tileMap.TilesLength;
        }
    }
}