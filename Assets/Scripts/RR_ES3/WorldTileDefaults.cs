using UnityEngine;
using UnityEngine.Tilemaps;

namespace RR_ES3
{
    public static class WorldTileDefaults
    {
        public static Color color => Color.white;
        public static Matrix4x4 transform => Matrix4x4.identity;
        public static TileFlags flags => TileFlags.LockColor;
        public static Tile.ColliderType colliderType => Tile.ColliderType.Sprite;
    }
}