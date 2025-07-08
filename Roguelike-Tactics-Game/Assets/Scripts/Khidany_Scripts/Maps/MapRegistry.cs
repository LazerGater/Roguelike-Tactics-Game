using System;
using System.Collections.Generic;

public static class MapRegistry
{
    public static readonly List<Func<MapData>> MapLoaders = new List<Func<MapData>>
    {
        //() => new MapData(
        //        SavedGridData.GridValues,
        //        SavedGridData.AllySpawnPoints,
        //        SavedGridData.PartyLimit),
        () => new MapData(
                PlainsCrossingGridData.GridValues,
                PlainsCrossingGridData.AllySpawnPoints,
                PlainsCrossingGridData.PartyLimit)
        // Add more here like "Mountain.GridValues,"
    };
}