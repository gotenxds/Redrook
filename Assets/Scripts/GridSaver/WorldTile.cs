using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace GridSaver
{
    public class WorldTile
    {
        [SerializeField]
        private Sprite sprite;
        [SerializeField] 
        private Color color;
        [SerializeField] 
        private Matrix4x4 transform;
        [SerializeField] 
        private TileFlags flags;
        [SerializeField] 
        private Tile.ColliderType colliderType;
        [SerializeField] 
        private Vector3Int position;

        public Sprite Sprite
        {
            get => sprite;
            set => sprite = value;
        }

        public Color Color
        {
            get => color;
            set => color = value;
        }

        public Matrix4x4 Transform
        {
            get => transform;
            set => transform = value;
        }

        public TileFlags Flags
        {
            get => flags;
            set => flags = value;
        }

        public Tile.ColliderType ColliderType
        {
            get => colliderType;
            set => colliderType = value;
        }

        public Vector3Int Position
        {
            get => position;
            set => position = value;
        }

        public static WorldTile Create(Tile tile, Vector3Int position)
        {
            var worldTile = new WorldTile
            {
                sprite = tile.sprite,
                color = tile.color,
                transform = tile.transform,
                flags = tile.flags,
                colliderType = tile.colliderType,
                position = position
            };

            return worldTile;
        }

        public Tile ToTile()
        {
            var tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = sprite;
            tile.color = color;
            tile.transform = transform;
            tile.flags = flags;
            tile.colliderType = colliderType;

            return tile;
        }
    }
}