using System.Collections.Generic;
using UnityEngine;

public class MapData
{
    public int[,] gridValues;
    public List<Vector2Int> allySpawns;
    public int partyLimit;                 

    public MapData(int[,] g, List<Vector2Int> s, int limit)  
    {
        gridValues = g;
        allySpawns = s;
        partyLimit = limit;
    }
}
