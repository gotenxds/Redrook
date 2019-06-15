using System.Collections.Generic;
using System.Linq;
using FlatBuffers;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GridSaver
{
    [ExecuteInEditMode]
    public class Saver: MonoBehaviour
    {
        public void Save()
        {
            var grid = GetComponent<Grid>();
            var writer = new Writer();
            var bounds = new BoundsInt(Vector3Int.zero, Configs.CellDimensions);   
            foreach (Transform child in grid.transform)
            {
                var tilemap = child.GetComponent<Tilemap>();

                foreach (var position in tilemap.cellBounds.allPositionsWithin)
                {
                    if (tilemap.HasTile(position))
                    {
                        var tile = (Tile) tilemap.GetTile(position);
                        writer.WriteTile(tile, position);
                    }
                }                
                
                writer.WriteTileMap();
            }
            
            writer.Save(grid.gameObject.scene.name);
            
            foreach (Transform child in grid.transform)
            {
             //   child.GetComponent<Tilemap>().ClearAllTiles();
            }
        }
    }
}