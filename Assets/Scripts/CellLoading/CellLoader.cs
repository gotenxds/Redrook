using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;
using World;
using Vector3 = UnityEngine.Vector3;

namespace CellLoading
{
    public class CellLoader : MonoBehaviour
    {
        [SerializeField] private Transform loadFrom;
        [SerializeField] private float loadRadius = Configs.CellSideSize / 300f + Configs.CellSideSize * 0.005f;
        [SerializeField] private float forgetRadius = Configs.CellSideSize / 2f + Configs.CellSideSize * 0.5f;

        private bool isLoading;
        private bool isUnloading;
        private Vector3 lastPosition;
        private readonly Dictionary<string, Cell> cellsByName = new Dictionary<string, Cell>();

        private Dictionary<string, Cell> CellsByName => cellsByName;

        public Cell? GetCellFor(Vector3 position)
        { 
            var cellLocation = Vector3Int.FloorToInt(position / Configs.CellSideSize);
            var cellName = CellUtils.CreateCellName(cellLocation);

            if (cellsByName.ContainsKey(cellName))
            {
                return cellsByName[cellName];
            }

            return null;
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

            var position = loadFrom.position / Configs.CellSideSize;
            
            Configs.Directions
                .Select(SurroundingCells(position))
                .Where(InLoadingRange(position))
                .Select(CellUtils.CreateCellName)
                .Where(NotLoaded)
                .Select(CellUtils.LoadCellByName).ToList()
                .ForEach(RegisterCell);
        }

        private static Func<(short, short), Vector3Int> SurroundingCells(Vector3 position)
        {
            return direction =>
                Vector3Int.RoundToInt(new Vector3(position.x + direction.Item1, position.y + direction.Item2));
        }

        private Func<Vector3Int, bool> InLoadingRange(Vector3 position)
        {
            return pos => Vector2.Distance((Vector2Int) pos, position) < loadRadius;
        }

        private bool NotLoaded(string name)
        {
            return !cellsByName.ContainsKey(name);
        }

        private void RegisterCell(Cell? cell)
        {
            if (cell.HasValue)
            {
                cellsByName.Add(cell.Value.Name, cell.Value);
            }
        }

        private IEnumerator UnloadCells()
        {
            if (isUnloading)
            {
                yield break;
            }
//
//            var operations = Enumerable
//                .Select(cellsByName.Values
//                    .Where(scene =>
//                        Vector2.Distance(scene.GetRootGameObjects()[0].transform.position, loadFrom.position) >
//                        forgetRadius), SceneManager.UnloadSceneAsync)
//                .Where(opp => opp != null)
//                .ToArray();
//
//            if (operations.Length <= 0) yield break;
//
//            isUnloading = true;
//            yield return new WaitUntil(() => operations.All(opp => opp.isDone));
//            isUnloading = false;
        }
    }
}