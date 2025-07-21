using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitMover))]
public class EnemyUnit : MonoBehaviour, IBattleUnit
{
    [Header("Data References")]
    public CharacterData baseCharacter;    // The generic enemy template (_phold)
    public ClassData classData;           // Random class assigned
    public UnitStats Stats { get; private set; }

    private GridMap grid;
    private Vector2Int gridPos;

    // Required by IBattleUnit
    public void SetGridPos(Vector2Int p) => gridPos = p;
    public Vector2Int GetGridPosition() => gridPos;

    public void Initialize(CharacterData cData, ClassData clData, GridMap g, Vector2Int startPos, int avgPartyLevel)
    {
        baseCharacter = cData;
        classData = clData;
        grid = g;
        gridPos = startPos;

        // Calculate enemy level: avg party level + 2
        int level = Mathf.Max(1, avgPartyLevel + 2);

        // Scale stats based on level and class
        Stats = CalculateEnemyStats(baseCharacter, classData, level);

        // Set world position
        transform.position = grid.GetWorldPosition(gridPos.x, gridPos.y) + Vector3.one * (grid.CellSize / 2f);

        // Mark tile as occupied
        if (!grid.TryMarkOccupied(gridPos))
        {
            Debug.LogError($"Enemy spawn clash at {gridPos}! Destroying self.");
            Destroy(gameObject);
        }

        // Init mover
        GetComponent<UnitMover>().Init(grid);
    }

    private UnitStats CalculateEnemyStats(CharacterData c, ClassData cl, int level)
    {
        // Start from base
        int hp = c.baseHP;
        int atk = c.baseAtk;
        int def = c.baseDef;
        int spd = c.baseSpeed;
        int luck = c.baseLuck;
        int dex = c.baseDex;

        // Apply scaling: (level - 1) * growth approximation
        int levelBonus = level - 1;
        hp += 2 * levelBonus;       // Example scaling
        atk += 1 * levelBonus;
        def += 1 * (levelBonus / 2);
        spd += 1 * (levelBonus / 2);
        luck += 1;
        dex += 1;

        // Apply class modifiers
        hp += cl.hpMod;
        atk += cl.atkMod;
        def += cl.defMod;
        spd += cl.speedMod;
        luck += cl.luckMod;
        dex += cl.dexMod;

        return new UnitStats
        {
            maxHP = hp,
            currentHP = hp,
            atk = atk,
            def = def,
            speed = spd,
            luck = luck,
            dex = dex,
            moveRange = cl.moveRange
        };
    }
}
