using System;

public static class MapSelector
{
    private static Random rng = new Random();

    public static int[,] GetRandomMap()
    {
        if (MapRegistry.MapLoaders.Count == 0)
        {
            throw new Exception("No maps registered in MapRegistry!");
        }

        int index = rng.Next(MapRegistry.MapLoaders.Count);
        return MapRegistry.MapLoaders[index]();
    }
}
