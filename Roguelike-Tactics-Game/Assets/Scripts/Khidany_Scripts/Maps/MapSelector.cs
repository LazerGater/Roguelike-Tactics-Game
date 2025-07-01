using System;

public static class MapSelector
{
    private static readonly Random rng = new Random();

    public static MapData GetRandomMap()
    {
        if (MapRegistry.MapLoaders.Count == 0)
            throw new Exception("No maps registered in MapRegistry!");

        int index = rng.Next(MapRegistry.MapLoaders.Count);
        return MapRegistry.MapLoaders[index]();
    }
}
