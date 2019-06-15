using System.Collections;
using System.Collections.Generic;
using CellLoading;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GridSaver
{
    public class Loader: MonoBehaviour
    {
        [SerializeField] private CellLoader cellLoader;
        [SerializeField] private Transform player;

        private readonly Vector3 boundingSize = new Vector3(35, 25);

        private TileCache tileCache;
        private Grid grid;
        private Vector2 lastPosition;
        private BoundsInt lastBounds;
        private string fromFile;
        
        private void Awake()
        {
            Reset();
            tileCache = TileCache.Instance;
            grid = GetComponent<Grid>();
        }

        public void Reset()
        {
            player = GameObject.FindWithTag("Player").transform;
        }

        public void LoadPlayerTiles()
        {
            var playerPos = (Vector2) grid.WorldToLocal(player.position);
            

            var bounds = GetBounds(playerPos);

            StartCoroutine(LoadTilesE(bounds));
        }
        
        private IEnumerator LoadTilesE(List<Vector3Int> bounds)
        {
            for (ushort i = 0; i < grid.transform.childCount; i++)
            {
                var tilemap = grid.transform.GetChild(i).GetComponent<Tilemap>();
                
                for (var index = 0; index < bounds.Count; index++)
                {
                    var position = bounds[index];

                    var cellOptional = cellLoader.GetCellFor(position);
                    
                    if (!cellOptional.HasValue) break;
                    
                    var tileRefIndex = cellOptional.Value.GetTileRefIndex(i, position);

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

        private List<Vector3Int> GetBounds(Vector2 playerPos)
        {
            var bounds = new Bounds(playerPos, boundingSize);
            var boundsMin = bounds.min;
            var boundsSize = bounds.size;
            var boundsInt = new BoundsInt((int) boundsMin.x, (int) boundsMin.y, (int) boundsMin.z, (int) boundsSize.x,
                (int) boundsSize.y, 1);

            var newPositions = new List<Vector3Int>();
            
            foreach (var position in boundsInt.allPositionsWithin)
            {
                if (!lastBounds.Contains(position) && position.x >= 0 && position.y >= 0)
                {
                    newPositions.Add(position);
                }
            }

            lastBounds = boundsInt;
            return newPositions;
        }

        private void Update()
        {
            if (player.position.Equals(lastPosition)) return;
            lastPosition = player.position;
            
            LoadPlayerTiles();
        }
    }
}