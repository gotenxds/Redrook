using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public class Vector3IntComparer : IComparer<Vector3Int>
    {
        public int Compare(Vector3Int v1, Vector3Int v2)
        {
            var y = v1.y - v2.y;
            if (y != 0)
            {
                return y;
            }
            return v1.x - v2.x;
        }
    }
}