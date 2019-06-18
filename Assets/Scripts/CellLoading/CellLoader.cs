using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;
using World;
using Color = UnityEngine.Color;
using Vector3 = UnityEngine.Vector3;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CellLoading
{
    [ExecuteAlways]
    public class CellLoader : MonoBehaviour
    {
        [SerializeField] private Transform loadFrom;
        [SerializeField] private float loadRadius = Configs.CellSideSize / 300f + Configs.CellSideSize * 0.005f;
        [SerializeField] private float forgetRadius = Configs.CellSideSize / 2f + Configs.CellSideSize * 0.5f;

        private bool isLoading;
        private bool isUnloading;
        private Vector3 lastPosition;
        private readonly Dictionary<string, CellWrapper> cellsByName = new Dictionary<string, CellWrapper>();

        private Dictionary<string, CellWrapper> CellsByName => cellsByName;

        public CellWrapper GetCellFor(Vector3 position)
        {
            var cellLocation = Vector3Int.FloorToInt(position / Configs.CellSideSize);
            var cellName = CellUtils.CreateCellName(cellLocation);

            if (cellsByName.ContainsKey(cellName))
            {
                return cellsByName[cellName];
            }

            return null;
        }

        public IEnumerable<CellWrapper> GetDirtyCells()
        {
            return cellsByName.Values.Where(cell => cell.IsDirty);
        }
        
        private void Start()
        {
            StartCoroutine(LoadCells());
        }

        private void LateUpdate()
        {
            if (loadFrom.position.Equals(lastPosition)) return;
            lastPosition = loadFrom.position;

            StartCoroutine(LoadCells());
            StartCoroutine(UnloadCells());
        }

        private IEnumerator LoadCells()
        {
            if (isLoading)
            {
                yield break;
            }

            var position = loadFrom.position;

            Configs.Directions
                .Select(SurroundingCells(position / Configs.CellSideSize))
                .Select(CellUtils.CreateCellName)
                .Where(InLoadingRange(position))
                .Where(NotLoaded)
                .Select(CellUtils.LoadCellByName).ToList()
                .ForEach(RegisterCell);
        }

        private static Func<(short, short), Vector3Int> SurroundingCells(Vector3 position)
        {
            return direction =>
                Vector3Int.RoundToInt(new Vector3(
                    position.x + direction.Item1,
                    position.y + direction.Item2));
        }

        private Func<string, bool> InLoadingRange(Vector3 pos)
        {
            return cellName => GetCellBounds(cellName, loadRadius).Contains(pos);
        }

        private bool NotLoaded(string name)
        {
            return !cellsByName.ContainsKey(name);
        }

        private void RegisterCell(CellWrapper cell)
        {
            if (cell != null)
            {
                cellsByName.Add(cell.Name, cell);
            }
        }

        private IEnumerator UnloadCells()
        {
            if (isUnloading)
            {
                yield break;
            }

            var position = loadFrom.position;

            var cells = cellsByName.Keys
                .Where(InUnLoadingRange(position))
                .Select(cellName => CellsByName[cellName]).ToList();

            foreach (var cell in cells)
            {
                cellsByName.Remove(cell.Name);
                Debug.Log("unloading");
            }

            //cellCleaner.Clean(cells);
        }

        private Func<string, bool> InUnLoadingRange(Vector3 pos)
        {
            return cellName => !GetCellBounds(cellName, forgetRadius).Contains(pos);
        }
        
        public Bounds GetCellBounds(string cellName, float offset)
        {
            return new Bounds(CellUtils.GetWorldCellCenter(cellName),
                new Vector3(Configs.CellSideSize * offset, Configs.CellSideSize * offset,
                    Configs.CellSideSize * offset));
        }
        
        private void OnDrawGizmos()
        {
            var camera = Camera.current;
            if (camera != null)
            {
                UnityEngine.Gizmos.color = cellsByName.Values.Any(cell => cell.IsDirty) ? Color.red : Color.green;
                var viewportToWorldPoint = camera.ViewportToWorldPoint(new Vector3(0.03f, .95f));
                
                UnityEngine.Gizmos.DrawSphere((Vector2)viewportToWorldPoint, HandleUtility.GetHandleSize(viewportToWorldPoint) * 0.3f);
                
                UnityEngine.Gizmos.color = Color.white;
            }
            UnityEngine.Gizmos.DrawWireCube(Configs.WorldCenter, new Vector3(Configs.WorldSideSizeInTiles, Configs.WorldSideSizeInTiles));

            var current = Vector3.zero;
            var limit = Configs.WorldSideSizeInTiles - Configs.CellSideSize;
            var offsetToCenter = (Vector3)Configs.CellDimensions / 2;
            while (current.x < limit || current.y < limit)
            {
                UnityEngine.Gizmos.DrawWireCube(current + offsetToCenter, (Vector3)Configs.CellDimensions);

                if (current.x < limit)
                {
                    current.x += Configs.CellSideSize;
                }
                else
                {
                    current.x = 0;
                    current.y += Configs.CellSideSize;
                }
            }
            UnityEngine.Gizmos.DrawWireCube(current + offsetToCenter, (Vector3)Configs.CellDimensions);
        }
    }
}