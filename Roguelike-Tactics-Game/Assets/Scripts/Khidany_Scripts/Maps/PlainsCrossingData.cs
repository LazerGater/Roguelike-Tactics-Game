using System.Collections.Generic;
using UnityEngine;

public static class PlainsCrossingGridData
{
    public static readonly int[,] GridValues = new int[,]
    {
        { 2, 2, 2, 2, 2, 1, 1, 1, 3, 3, 1, 1, 1, 1, 1 },
        { 2, 1, 1, 1, 1, 1, 1, 1, 3, 3, 1, 1, 1, 1, 1 },
        { 1, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
        { 1, 1, 1, 2, 2, 1, 1, 1, 3, 3, 1, 1, 1, 1, 1 },
        { 1, 1, 1, 1, 1, 2, 1, 3, 3, 3, 1, 1, 2, 2, 2 },
        { 1, 1, 1, 1, 1, 1, 1, 3, 3, 3, 2, 2, 2, 4, 4 },
        { 2, 1, 1, 1, 1, 1, 1, 3, 3, 2, 4, 4, 4, 4, 99 },
        { 1, 1, 1, 1, 1, 1, 1, 3, 3, 1, 2, 2, 2, 4, 4 },
        { 1, 1, 1, 1, 1, 1, 1, 3, 3, 1, 1, 1, 2, 2, 2 },
        { 1, 1, 1, 1, 1, 1, 3, 3, 3, 1, 1, 1, 1, 1, 1 },
        { 1, 1, 1, 1, 2, 1, 3, 3, 1, 2, 1, 1, 1, 1, 1 },
        { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2 },
        { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
        { 1, 1, 1, 1, 1, 1, 3, 3, 1, 1, 1, 1, 99, 1, 99 },
        { 1, 1, 1, 1, 1, 1, 3, 3, 1, 1, 1, 1, 99, 99, 99 }
    };

    // Ally Spawn Points
    public static readonly List<Vector2Int> AllySpawnPoints = new()
    {
        new Vector2Int(2, 0),
        new Vector2Int(1, 2),
        new Vector2Int(3, 2),
        new Vector2Int(0, 1),
        new Vector2Int(4, 1)

    };

    // Enemy Spawn Points
    public static readonly List<Vector2Int> EnemySpawnPoints = new()
    {
        new Vector2Int(11, 4),
        new Vector2Int(7, 12), 
        new Vector2Int(11, 11),
        new Vector2Int(14, 11),
        new Vector2Int(13, 11),
        new Vector2Int(2, 7),
        new Vector2Int(4, 6),
        new Vector2Int(13, 4),
        new Vector2Int(10, 3),
        new Vector2Int(9, 1)
    };

    // Commander Spawn Point
    public static readonly Vector2Int CommanderTile = new(13, 13);

    public const int PartyLimit = 5;  

    // Fort tile – treated as plain grass for now.
    public static readonly Vector2Int FortTile = new Vector2Int(7, 12);

    // Village tile – treated as plain grass for now.
    public static readonly Vector2Int VisitTile = new(13, 1);
}
