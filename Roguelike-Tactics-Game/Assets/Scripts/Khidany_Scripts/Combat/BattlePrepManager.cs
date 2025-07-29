using System.Collections.Generic;
using UnityEngine;

public class BattlePrepManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GridInitializer gridInitializer; // Assign in Inspector
    [SerializeField] private GridManager gridManager; // Assign in Inspector
    [SerializeField] private EnemySpawner enemySpawner;

    private List<Vector2Int> startingTiles;
    private int partyLimit;

    private void Start()
    {
        if (gridInitializer == null || gridManager == null || enemySpawner == null)
        {
            Debug.LogError("BattlePrepManager: Missing required references!");
            return;
        }

        // 1) Pull map data from GridInitializer
        startingTiles = new List<Vector2Int>(gridInitializer.AllySpawns);
        partyLimit = gridInitializer.PartyLimit;

        // 2) Spawn player units
        gridManager.SpawnSelectedParty(startingTiles, false);

        // 3) Spawn enemies
        enemySpawner.SpawnEnemyParty(gridInitializer);

        // 4) Begin turn loop
        TurnManager.Instance.StartBattle();
    }
}
