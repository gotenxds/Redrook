using System;
using System.Collections.Generic;
using UnityEngine;

public static class Configs
{
    public static int WorldSideSizeInTiles => 8000;
    
    public static int WorldTileCount => WorldSideSizeInTiles * WorldSideSizeInTiles;
    
    public static int CellSideSize => 1000;    

    public static Vector3Int CellDimensions => new Vector3Int(CellSideSize, CellSideSize, 1);
    
    public static int CellSize => CellDimensions.x * CellDimensions.y;


    public static int WorldCellCount => WorldTileCount / CellSize;

    public static ushort PositionToIndex(Vector3Int position)
    {
        return (ushort) (position.x + position.y * WorldSideSizeInTiles);
    }
    
    public static ushort CellPositionToIndex(Vector3Int position)
    {
        return (ushort) (position.x + position.y * CellDimensions.x);
    }

    public static Vector3 CellSizeVector => new Vector3(CellSideSize, CellSideSize, 1);
    
    public static Vector3Int CellSizeVectorInt => Vector3Int.RoundToInt(CellSizeVector);
    
    public static readonly List<(short, short)> Directions = new List<(short, short)>
    {
        (1, 0), (1, 1), (1, -1),
        (-1, 0), (-1, 1), (-1, -1),
        (0, 0), (0, 1), (0, -1)
    };
    
    public static readonly Vector2Int[] VectorDirections = {
        new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int(1, 1),
        new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0),
        new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1)
    };
}