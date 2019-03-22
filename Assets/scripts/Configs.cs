using System.Collections.Generic;

public static class Configs
{
    public static int CellSize => 100;
    public static readonly List<(short, short)> Directions = new List<(short, short)>
    {
        (1, 0), (1, 1), (1, -1),
        (-1, 0), (-1, 1), (-1, -1),
        (0, 0), (0, 1), (0, -1)
    };
}