using System.Collections;
using System.Collections.Generic;
using CellLoading;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GridSaver
{
    [ExecuteAlways]
    public class Loader: MonoBehaviour
    {
        [SerializeField] private CellLoader cellLoader;
        [SerializeField] private Transform player;

        [SerializeField]
        private Vector3 editModeBoundingSize = new Vector3(100, 100);
        
        private readonly Vector3 boundingSize = new Vector3(35, 25);

        private TileCache tileCache = TileCache.Instance;
        private Grid grid;
        private Vector2 lastPosition;
        private BoundsInt lastBounds;
        private string fromFile;

        private void Awake()
        {
            Reset();
            grid = GetComponent<Grid>();
        }

        public void Reset()
        {
            player = GameObject.FindWithTag("Player").transform;
        }

        public void LoadPlayerTiles()
        {
            var playerPos = (Vector2) grid.WorldToLocal(player.position);
            

            var (newPositions, positionsToClean) = GetBounds(playerPos);

            StartCoroutine(LoadTilesE(newPositions));
            if (Application.isPlaying)
            {
                StartCoroutine(CleanE(positionsToClean));
            }
        }
        
        private IEnumerator LoadTilesE(List<Vector3Int> bounds)
        {
            for (ushort i = 0; i < grid.transform.childCount; i++)
            {
                var tilemap = grid.transform.GetChild(i).GetComponent<Tilemap>();
                
                for (var index = 0; index < bounds.Count; index++)
                {
                    var position = bounds[index];

                    var cell = cellLoader.GetCellFor(position);
                    
                    if (cell == null) break;
                    
                    var tileRefIndex = cell.GetTileRefIndex(i, position);

                    if (tileRefIndex != ushort.MaxValue)
                    {
                        var tile = tileCache.GetTile(tileRefIndex);

                        tilemap.SetTile(position, tile);
                    }

                    if (index % 1000 == 0)
                    {
                        yield return new WaitForEndOfFrame();                        
                    }
                }
            }
        }
        private IEnumerator CleanE(List<Vector3Int> bounds)
        {
            for (ushort i = 0; i < grid.transform.childCount; i++)
            {
                var tilemap = grid.transform.GetChild(i).GetComponent<Tilemap>();
                
                for (var index = 0; index < bounds.Count; index++)
                {
                    var position = bounds[index];

                    tilemap.SetTile(position, null);
                    
                    if (index % 1000 == 0)
                    {
                        yield return new WaitForEndOfFrame();                        
                    }
                }
            }
        }

        private (List<Vector3Int>, List<Vector3Int>) GetBounds(Vector2 playerPos)
        {
            var bounds = new Bounds(playerPos, Application.isPlaying ? boundingSize : editModeBoundingSize);
            var boundsMin = bounds.min;
            var boundsSize = bounds.size;
            var boundsInt = new BoundsInt((int) boundsMin.x, (int) boundsMin.y, (int) boundsMin.z, (int) boundsSize.x,
                (int) boundsSize.y, 1);

            var newPositions = new List<Vector3Int>();
            var positionsToClean = new List<Vector3Int>();
            
            var newBounding = boundsInt.allPositionsWithin;
            var lastBounding = lastBounds.allPositionsWithin;

            do
            {
                if (!boundsInt.Contains(lastBounding.Current))
                {
                    positionsToClean.Add(lastBounding.Current);                    
                }

                var position = newBounding.Current;
                if (!lastBounds.Contains(position) && position.x >= 0 && position.y >= 0)
                {
                    newPositions.Add(position);
                }

            } while (newBounding.MoveNext() || lastBounding.MoveNext());

            lastBounds = boundsInt;
            return (newPositions, positionsToClean);
        }

        private void Update()
        {
            if (player.position.Equals(lastPosition)) return;
            lastPosition = player.position;
            
            LoadPlayerTiles();
        }
    }
}