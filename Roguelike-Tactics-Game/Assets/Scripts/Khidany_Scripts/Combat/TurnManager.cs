using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum TurnPhase
{
    Player,
    Enemy
}

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    private int turnNumber = 1;
    public int CurrentTurn => turnNumber;
    public bool IsBattleActive { get; private set; }

    private List<PlayerUnit> playerUnits = new List<PlayerUnit>();
    private List<EnemyUnit> enemyUnits = new List<EnemyUnit>();

    public TurnPhase CurrentPhase { get; private set; } = TurnPhase.Player;

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        Instance = this;
    }

    public void RegisterPlayerUnit(PlayerUnit unit)
    {
        playerUnits.Add(unit);
        Debug.Log($"PlayerUnit registered: {unit.name}. Total: {playerUnits.Count}");
        PrintCurrentUnits();
    }

    public void RegisterEnemyUnit(EnemyUnit unit)
    {
        enemyUnits.Add(unit);
        Debug.Log($"EnemyUnit registered: {unit.name}. Total: {enemyUnits.Count}");
        PrintCurrentUnits();
    }

    public void NotifyUnitActed()
    {
        if (CurrentPhase != TurnPhase.Player) return;

        foreach (var unit in playerUnits)
        {
            if (!unit.HasActed)
                return;
        }

        Debug.Log("All player units have acted.");
        StartCoroutine(HandleEnemyPhase());
    }

    private IEnumerator HandleEnemyPhase()
    {
        CurrentPhase = TurnPhase.Enemy;
        Debug.Log("== Enemy Phase ==");

        // Simulate enemy delay
        yield return new WaitForSeconds(1f);

        // End enemy phase, go to next turn
        AdvanceTurn();
    }

    private void AdvanceTurn()
    {
        turnNumber++;
        Debug.Log($"== Turn {turnNumber} / Player Phase ==");

        CurrentPhase = TurnPhase.Player;

        foreach (var unit in playerUnits)
        {
            unit.ResetTurn();
        }
    }

    public void StartBattle()
    {
        IsBattleActive = true;
        turnNumber = 0;
        AdvanceTurn();
    }

    private void PrintCurrentUnits()
    {
        string players = playerUnits.Count > 0 ? string.Join(", ", playerUnits.ConvertAll(u => u.name)) : "None";
        string enemies = enemyUnits.Count > 0 ? string.Join(", ", enemyUnits.ConvertAll(u => u.name)) : "None";

        Debug.Log($"[Unit Status] Players: {players} | Enemies: {enemies}");
    }
}