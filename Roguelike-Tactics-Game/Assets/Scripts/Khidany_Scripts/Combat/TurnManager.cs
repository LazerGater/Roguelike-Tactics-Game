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

    private List<PlayerUnit> playerUnits = new List<PlayerUnit>();

    public TurnPhase CurrentPhase { get; private set; } = TurnPhase.Player;

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        Instance = this;
    }

    public void RegisterPlayerUnit(PlayerUnit unit)
    {
        playerUnits.Add(unit);
    }

    public void NotifyUnitActed()
    {
        if (CurrentPhase != TurnPhase.Player) return;

        foreach (var unit in playerUnits)
        {
            if (!unit.HasActed)
                return;
        }

        // All player units have acted -> go to Enemy phase
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
        turnNumber = 0;
        AdvanceTurn();
    }
}
