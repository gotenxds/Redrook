using System;
using UnityEditor.Tilemaps;
using UnityEngine;

namespace TileBrushes
{
    [CustomGridBrush(false, true, false, "RedRook Default brush")]
    public class RedRookDefaultBrush: RedRookBrushWrapper 
    {
        private new void OnEnable()
        {
            brush = CreateInstance<GridBrush>();
        }
    }
}