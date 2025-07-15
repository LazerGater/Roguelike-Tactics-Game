using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using Unity.Properties;
using System.Collections.Generic;
using UnityEngine.Timeline;

// TODO: make it so the markers arent fully randomized, like you always get a town in the middle, or you always get some healing at the end, etc.
// TODO: draw lines between markers

public class RandomSpawner : MonoBehaviour
{
    public GameObject[] mapMarkers;
    public GameObject startMarkers;
    public GameObject endMarkers;
    public GameObject linePrefab;

    private List<GameObject> spawnedMarkers = new List<GameObject>();
    private List<(Vector2 start, Vector2 end)> existingLines = new List<(Vector2, Vector2)>();

    // Mark Bounds for Spawning and interval between spawns
    public float    leftBound = -6.5f,
                    rightBound = 6.5f,
                    interval = 2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Start the spawning coroutine
        StartCoroutine(SpawnMarkersWithDelay());
    } // end start


    // Update is called once per frame
    void Update()
    {
    } // end update

    /// <summary>
    /// Spawns in Markers for Overworld Map
    /// Steps of Process
    ///     1. Set x bounds for Spawning and iterate every interval of it from left to right
    ///     2. set y bounds for spawning
    ///     3. Find a Valid Place to Spawn Marker 
    ///     4. Spawn Marker
    ///     5. Move to next X Spawn
    ///     6. Generate Lines
    /// </summary>
    IEnumerator SpawnMarkersWithDelay()
    {
        spawnedMarkers.Add(startMarkers);

        // 1. Set x bounds for Spawning and iterate every interval of it from left to right
        for (float distance = leftBound; distance < rightBound; distance += interval)
        {
            int spawnAmount = Random.Range(0, 4);
            List<Vector2> spawnedPositions = new List<Vector2>();

            // 2. set y bounds for spawning
            for (int i = 0; i < spawnAmount; i++)
            {
                int attempts = 0;
                bool validPositionFound = false;
                Vector2 randomSpawnPosition = Vector2.zero;

                // 3. Find a Valid Place to Spawn Marker 
                while (attempts < 10 && !validPositionFound)
                {
                    randomSpawnPosition = new Vector2(
                        Random.Range(distance - 0.05f, distance + 0.05f),
                        Random.Range(-4f, 4.5f)
                    );

                    validPositionFound = true;

                    foreach (Vector2 pos in spawnedPositions)
                    {
                        if (Vector2.Distance(pos, randomSpawnPosition) < 2f)
                        {
                            validPositionFound = false;
                            break;
                        }
                    }

                    attempts++;
                }

                // 4. Spawn Marker
                if (validPositionFound)
                {
                    spawnedPositions.Add(randomSpawnPosition);
                    // Guide the Randomization
                    RandomizationGuide(randomSpawnPosition);
                    yield return new WaitForSecondsRealtime(0.2f);
                }
            }

            // 5. Move to next X Spawn
            if (spawnAmount > 0)
                yield return new WaitForSecondsRealtime(0.5f);
        }

        // 6. Generate Lines Between Markers
        spawnedMarkers.Add(endMarkers);
        StartCoroutine(GenerateLinesBetweenMarkers());
    } // end SpawnMarkersWithDelay

    void RandomizationGuide(Vector2 randomSpawnPosition)
    {
        int rand;
        GameObject marker;

        // Start is Always Battle
        if (randomSpawnPosition.x < leftBound + interval / 1.5)
        {
            marker = Instantiate(mapMarkers[0], randomSpawnPosition, Quaternion.identity);
        }

        // end is always village
        else if (randomSpawnPosition.x > rightBound - interval / 1.5)
        {
            marker = Instantiate(mapMarkers[4], randomSpawnPosition, Quaternion.identity);
        }

        // Middle is always shop, village, or event (skewed more towards event)
        else if ((randomSpawnPosition.x > 0 - interval / 1.5) && (randomSpawnPosition.x < 0 + interval / 1.5))
        {
            rand = Random.Range(1, 4);
            if (rand == 2) rand--;
            marker = Instantiate(mapMarkers[rand], randomSpawnPosition, Quaternion.identity);
        }

        // Right Middle is always battle, event or mystery
        else if (randomSpawnPosition.x > 0 + interval / 1.5)
        {
            marker = Instantiate(mapMarkers[Random.Range(0, 2)], randomSpawnPosition, Quaternion.identity);
        }

        // The Rest is all but villages, and skewed more towards battle
        else
        {
            rand = Random.Range(0, 4);
            if (rand == 4) rand = 0;
            marker = Instantiate(mapMarkers[rand], randomSpawnPosition, Quaternion.identity);
            print("THE REST " + randomSpawnPosition + "   " + marker);
        }
        spawnedMarkers.Add(marker);
    } // end RandomizationGuide

    /// <summary>
    ///     After Markers are spawned on map, run this to generate roads between them
    ///         + It takes in a list of markers then creates lines between them from left to right
    /// </summary>
    IEnumerator GenerateLinesBetweenMarkers()
    {
        foreach (GameObject marker in spawnedMarkers)
        {
            Vector2 markerPos = marker.transform.position;

            // Find candidates to the right
            List<GameObject> rightSideMarkers = new List<GameObject>();

            foreach (GameObject other in spawnedMarkers)
            {
                if (other == marker) continue;

                Vector2 otherPos = other.transform.position;
                if (otherPos.x > markerPos.x)
                {
                    rightSideMarkers.Add(other);
                }
            }

            // Sort by distance
            rightSideMarkers.Sort((a, b) =>
                Vector2.Distance(markerPos, a.transform.position)
                .CompareTo(Vector2.Distance(markerPos, b.transform.position))
            );

            // take the lower of 2 randomizations
            int connections = Random.Range(1, Mathf.Min(3, rightSideMarkers.Count + 1));
            connections = Mathf.Min(Random.Range(1, Mathf.Min(3, rightSideMarkers.Count + 1)), connections);

            int addedConnections = 0;
            int index = 0;
            while (addedConnections < connections && index < rightSideMarkers.Count)
            {
                GameObject target = rightSideMarkers[index];
                Vector2 targetPos = target.transform.position;

                // If a line would intersect, dont create the line
                bool intersects = false;
                foreach (var line in existingLines)
                {
                    if (true == LinesIntersect(markerPos, targetPos, line.start, line.end))
                    {
                        intersects = true;
                        break;
                    }
                }

                // Create Line between 2 points
                if ( (false == intersects) && (Mathf.Abs(markerPos.x - rightSideMarkers[index].transform.position.x) > 0.5) )
                {
                    CreateLine(markerPos, targetPos);
                    existingLines.Add((markerPos, targetPos));
                    addedConnections++;
                    yield return new WaitForSecondsRealtime(0.5f);
                }

                index++;
            }
        } // end foreach
    } // end GenerateLinesBetweenMarkers

    /// <summary>
    ///     Creates a line with a start vector and end vector
    /// </summary>
    /// <param Start of Line = "start"></param>
    /// <param End of Line   = "end"></param>
    void CreateLine(Vector2 start, Vector2 end)
    {
        GameObject lineObj = Instantiate(linePrefab);
        LineRenderer lr = lineObj.GetComponent<LineRenderer>();

        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    } // end CreateLine

    /// <summary>
    ///     Checks if lines intersect, if they do, doesn't lay road.
    /// </summary>
    /// <param Start of Line 1 = "a1"></param>
    /// <param End of Line 2   = "a2"></param>
    /// <param Start of Line 2 = "b1"></param>
    /// <param End of Line 2   = "b2"></param>
    /// <returns></returns>
    bool LinesIntersect(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
    {
        // Check for shared endpoints — allow if they touch at endpoints
        if (a1 == b1 || a1 == b2 || a2 == b1 || a2 == b2)
            return false;

        float d = (a2.x - a1.x) * (b2.y - b1.y) - (a2.y - a1.y) * (b2.x - b1.x);
        if (d == 0) return false; // parallel or colinear

        float u = ((b1.x - a1.x) * (b2.y - b1.y) - (b1.y - a1.y) * (b2.x - b1.x)) / d;
        float v = ((b1.x - a1.x) * (a2.y - a1.y) - (b1.y - a1.y) * (a2.x - a1.x)) / d;

        return (u >= 0 && u <= 1) && (v >= 0 && v <= 1);
    } // end LineIntersect

} // end RandomSpawner


