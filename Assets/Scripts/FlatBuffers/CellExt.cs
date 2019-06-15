using UnityEngine;
using UnityEngine.Tilemaps;
using Utils;

namespace World
{
    public partial struct Cell
    {
        public string Name
        {
            get => name;
            set
            {
                name = value;
                cellPosition = CellUtils.GetCellPosition(name);
            }
        }

        public string Path { get; set; }

        private Vector3Int cellPosition;
        private string name;

        public ushort GetTileRefIndex(ushort mapIndex, Vector3Int worldPosition)
        {
            var localPosition = worldPosition - cellPosition;
            var index = localPosition.x + localPosition.y * Configs.CellSideSize;

            var tileMap = TileMaps(mapIndex).Value;
            return tileMap.Tiles(index + tileMap.Offset);
        }
    }
}