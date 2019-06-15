using RR_ES3;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace World
{
    public class Utils
    {
        public static UnityEngine.Vector3Int ToPosition(Vector3 vec)
        {
            return new UnityEngine.Vector3Int((int) vec.X, (int) vec.Y, (int) vec.Z);
        }

        public static Tile ToTile(WorldTile worldTile, Sprite sprite)
        {
            var tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = sprite;
            tile.color = WorldTileDefaults.color;
            tile.transform = Matrix4x4.identity;
            tile.flags = WorldTileDefaults.flags;
            tile.colliderType = WorldTileDefaults.colliderType;

            return tile;
        }

        public static UnityEngine.Color ToColor(Color color)
        {
            return new UnityEngine.Color(color.Red, color.Green, color.Blue, color.Alpha);
        }
        
    }
}