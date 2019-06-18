using System;
using CellLoading;
using FlatBuffers;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TileBrushes
{
    [CustomGridBrush(true, false, false, "RedRookWrapperBrush")]
    public class RedRookBrushWrapper: GridBrush 
    {
        protected GridBrush brush;

        private void CheckForDirtyness(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            ushort index = 0;
            ushort tileRef = UInt16.MaxValue;
            foreach (var t in gridLayout.transform)
            {
                var tilemap = ((Transform) t).GetComponent<Tilemap>();
                if (tilemap.Equals(brushTarget.GetComponent<Tilemap>()))
                {
                    var tileBase = tilemap.GetTile(position);
                    if (tileBase != null)
                    {
                        tileRef = TileReferenceFile.Instance.GetRefOf(tileBase.name);
                    }

                    break;
                }

                index++;
            }
            
            var cell = GameObject.FindWithTag("Root").GetComponent<CellLoader>().GetCellFor(position);
            if (cell == null) return;
                        
            cell.MarkDirty(index, position, tileRef);
        }

        private void CheckForDirtyness(GridLayout gridLayout, GameObject brushTarget, BoundsInt bounds)
        {
            foreach (var position in bounds.allPositionsWithin)
            {    
                CheckForDirtyness(gridLayout, brushTarget, position);
            }
        }

        public override void Paint(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            base.Paint(gridLayout, brushTarget, position);
            CheckForDirtyness(gridLayout, brushTarget, position);
        }

        public override void Erase(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            base.Erase(gridLayout, brushTarget, position);
            CheckForDirtyness(gridLayout, brushTarget, position);
        }

        public override void BoxFill(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            base.BoxFill(gridLayout, brushTarget, position);
            CheckForDirtyness(gridLayout, brushTarget, position);
        }

        public override void BoxErase(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            base.BoxErase(gridLayout, brushTarget, position);
            CheckForDirtyness(gridLayout, brushTarget, position);
        }

        public override void FloodFill(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            base.FloodFill(gridLayout, brushTarget, position);
            CheckForDirtyness(gridLayout, brushTarget, position);
        }
    }
}