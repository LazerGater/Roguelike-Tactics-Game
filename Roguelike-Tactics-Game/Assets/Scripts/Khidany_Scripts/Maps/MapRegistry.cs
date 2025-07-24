using System;
using System.Collections.Generic;

public static class MapRegistry
{
    public static readonly List<Func<MapData>> MapLoaders = new List<Func<MapData>>
    {
        () => new MapData(
                PlainsCrossingGridData15x15.GridValues,
                PlainsCrossingGridData15x15.AllySpawnPoints,
                PlainsCrossingGridData15x15.EnemySpawnPoints,
                PlainsCrossingGridData15x15.CommanderTile,
                PlainsCrossingGridData15x15.PartyLimit)
    };
    // Add more here like "Mountain.GridValues,"
}