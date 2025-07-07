using System.Collections.Generic;
using UnityEngine;

public class PlayerPathfinder : MonoBehaviour
{
    // Returns the cheapest path from start -> goal (inclusive) OR null if impossible.
    // Movement costs come from grid.GetValue(x,y). Occupied tiles are blocked.
    public static List<Vector2Int> FindPath(
        GridMap grid,
        Vector2Int start,
        Vector2Int goal,
        int maxMovePoints)
    {
        // Dijkstra (uniform queue = breadth first by accumulated cost)
        var frontier = new Queue<Vector2Int>();
        var cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        var costSoFar = new Dictionary<Vector2Int, int>();

        frontier.Enqueue(start);
        costSoFar[start] = 0;

        Vector2Int[] DIRS = {
            new Vector2Int( 1, 0), new Vector2Int(-1, 0),
            new Vector2Int( 0, 1), new Vector2Int( 0,-1)
        };

        while (frontier.Count > 0)
        {
            var current = frontier.Dequeue();
            if (current == goal) break;

            foreach (var dir in DIRS)
            {
                var next = current + dir;
                if (!grid.IsInBounds(next.x, next.y)) continue;
                if (grid.IsOccupied(next)) continue;

                int tileCost = grid.GetValue(next.x, next.y);
                if (tileCost <= 0) continue;

                int newCost = costSoFar[current] + tileCost;
                if (newCost > maxMovePoints) continue;

                if (costSoFar.TryGetValue(next, out int saved) && saved <= newCost)
                    continue;

                costSoFar[next] = newCost;
                cameFrom[next] = current;
                frontier.Enqueue(next);
            }
        }

        if (!cameFrom.ContainsKey(goal)) return null; // unreachable

        // Reconstruct path (goal -> start), then reverse.
        var path = new List<Vector2Int> { goal };
        var step = goal;
        while (step != start)
        {
            step = cameFrom[step];
            path.Add(step);
        }
        path.Reverse();
        return path;
    }
}
