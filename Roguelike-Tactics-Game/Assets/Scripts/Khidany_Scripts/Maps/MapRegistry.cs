using System;
using System.Collections.Generic;

public static class MapRegistry
{
    public static readonly List<Func<int[,]>> MapLoaders = new List<Func<int[,]>>()
    {
        () => SavedGridData.GridValues
        // Add more here like "Mountain.GridValues,"
    };
}