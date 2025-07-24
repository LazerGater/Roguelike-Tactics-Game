using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance { get; private set; }

    [Header("References")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private CharacterData genericEnemyTemplate;

    [Header("Enemy Pool")]
    [SerializeField] private List<ClassData> enemyClassPool = new List<ClassData>();

    [Header("Commander Settings")]
    [SerializeField] private bool spawnCommander = true; // Replaces allowCommander
    [SerializeField] private GameObject commanderPrefab;
    [SerializeField] private CharacterData commanderCharacter;
    [SerializeField] private int commanderLevelOffset = 4;

    private GridMap grid;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Spawns enemies and commander based on the active map from GridInitializer.
    /// </summary>
    public void SpawnEnemyParty(GridInitializer initializer)
    {
        if (initializer == null)
        {
            Debug.LogError("EnemySpawner: GridInitializer is null!");
            return;
        }

        grid = initializer.Grid;

        // Calculate average player level
        int avgPartyLevel = CalculateAveragePlayerLevel();
        Debug.Log($"Average Party Level: {avgPartyLevel}");

        // 1. Spawn all regular enemies
        foreach (var pos in initializer.EnemySpawns)
        {
            if (grid.IsOccupied(pos)) continue;

            ClassData chosenClass = GetRandomEnemyClass();
            SpawnEnemy(genericEnemyTemplate, chosenClass, pos, avgPartyLevel + 2);
        }

        // 2. Spawn commander if settings allow and map defines a commander tile
        if (spawnCommander && initializer.CommanderTile.HasValue)
        {
            Vector2Int pos = initializer.CommanderTile.Value;
            if (!grid.IsOccupied(pos))
            {
                GameObject commanderObj = Instantiate(commanderPrefab);
                EnemyUnit commanderUnit = commanderObj.GetComponent<EnemyUnit>();

                int commanderLevel = avgPartyLevel + commanderLevelOffset;
                commanderUnit.Initialize(commanderCharacter, GetRandomEnemyClass(), grid, pos, commanderLevel);

                TurnManager.Instance.RegisterEnemyUnit(commanderUnit);
                grid.MarkOccupied(pos);

                Debug.Log($"Commander spawned at: {pos}");
            }
        }
    }

    private void SpawnEnemy(CharacterData baseData, ClassData classData, Vector2Int gridPos, int level)
    {
        GameObject enemyObj = Instantiate(enemyPrefab);
        EnemyUnit enemy = enemyObj.GetComponent<EnemyUnit>();

        if (enemy == null)
        {
            Debug.LogError("Enemy prefab is missing EnemyUnit component!");
            Destroy(enemyObj);
            return;
        }

        enemy.Initialize(baseData, classData, grid, gridPos, level);
        TurnManager.Instance.RegisterEnemyUnit(enemy);
        grid.MarkOccupied(gridPos);

        Debug.Log($"Enemy spawned at {gridPos} as {classData.className}, Level {level}");
    }

    private ClassData GetRandomEnemyClass()
    {
        if (enemyClassPool == null || enemyClassPool.Count == 0)
        {
            Debug.LogError("EnemySpawner: Enemy class pool is empty!");
            return null;
        }
        return enemyClassPool[Random.Range(0, enemyClassPool.Count)];
    }

    private int CalculateAveragePlayerLevel()
    {
        var party = PartyCarrier.Instance.playerParty;
        if (party == null || party.Count == 0) return 1;

        float sum = party.Sum(m => m.character.baseLevel);
        return Mathf.CeilToInt(sum / party.Count);
    }
}
