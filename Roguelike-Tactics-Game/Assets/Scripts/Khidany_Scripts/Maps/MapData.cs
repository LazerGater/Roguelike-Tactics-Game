using System.Collections.Generic;
using UnityEngine;

public class MapData
{
    public int[,] gridValues;                     // Movement cost grid
    public List<Vector2Int> allySpawns;          // Player starting tiles
    public List<Vector2Int> enemySpawns;         // Enemy starting tiles
    public Vector2Int? commanderTile;            // Optional commander tile
    public int partyLimit;                       // Max player units

    public MapData(int[,] g, List<Vector2Int> ally, List<Vector2Int> enemy, Vector2Int? commander, int limit)
    {
        gridValues = g;
        allySpawns = ally;
        enemySpawns = enemy;
        commanderTile = commander;              // Can be null if map has no commander
        partyLimit = limit;
    }
}
