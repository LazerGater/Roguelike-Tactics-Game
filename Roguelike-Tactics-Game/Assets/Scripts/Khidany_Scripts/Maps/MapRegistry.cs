using System;
using System.Collections.Generic;

public static class MapRegistry
{
    public static readonly List<Func<MapData>> MapLoaders = new List<Func<MapData>>
    {
        () => new MapData(
                SavedGridData.GridValues,
                SavedGridData.AllySpawnPoints,
                SavedGridData.PartyLimit) 
        // Add more here like "Mountain.GridValues,"
    };
}