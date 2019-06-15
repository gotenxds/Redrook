using System;
using System.Linq;
using FlatBuffers;
using RR_ES3;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utils;
using Random = System.Random;

namespace DefaultNamespace
{
    public class GenerateWorldMap: MonoBehaviour
    {
        [SerializeField] private Tile[] tiles;
        private Random random = new Random();
           
        public void Generate()
        {
            var grid = GetComponent<Grid>();
            var sideLength = (int)Math.Sqrt(Configs.WorldCellCount);
            
            var worldBounds = new BoundsInt(Vector3Int.zero, new Vector3Int(sideLength, sideLength, 1));
            var cellBounds = new BoundsInt(Vector3Int.zero, Configs.CellDimensions);
            foreach (var worldPosition in worldBounds.allPositionsWithin)
            {
                if (worldPosition.z > 0) break;
                
                var writer = new Writer();
                foreach (Transform child in grid.transform)
                {
                    foreach (var position in cellBounds.allPositionsWithin)
                    {
                        if (position.z > 0) break;

                        writer.WriteTile(GenerateRandomTile(), position);
                    }                
                
                    writer.WriteTileMap();
                }
                
                writer.Save(CellUtils.CreateCellName(worldPosition));
            }

            Debug.Log("Done mofo!");
        }

        private Tile GenerateRandomTile()
        {
            return tiles[random.Next(0, tiles.Length - 1)];
        }
    }
}