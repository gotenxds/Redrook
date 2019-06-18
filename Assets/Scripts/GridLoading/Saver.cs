using System.Linq;
using CellLoading;
using FlatBuffers;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GridSaver
{
    [ExecuteInEditMode]
    public class Saver: MonoBehaviour
    {
        public void Save()
        {
            var cellLoader = GameObject.FindWithTag("Root").GetComponent<CellLoader>();
            var cells = cellLoader.GetDirtyCells();
            var grid = GetComponent<Grid>();
            
            
            
            foreach (var cell in cells)
            {
                var writer = new Writer();
                ushort mapIndex = 0;
                foreach (Transform child in grid.transform)
                {
                    var tilemap = child.GetComponent<Tilemap>();
                    var vector3Ints = cell.GetBounds(mapIndex).ToArray();

                    foreach (var positionIndex in cell.GetAllTilesLocations(mapIndex))
                    {
                        var position = vector3Ints[positionIndex];
                        
                        if (tilemap.HasTile(position))
                        {
                            var tile = (Tile) tilemap.GetTile(position);
                            writer.WriteTile(tile, position);
                        }
                    }                
                
                    writer.WriteTileMap();
                    mapIndex++;
                }
                
                writer.Save(cell.Name);
            }

            foreach (Transform child in grid.transform)
            {
                child.GetComponent<Tilemap>().ClearAllTiles();
            }
        }
    }
}