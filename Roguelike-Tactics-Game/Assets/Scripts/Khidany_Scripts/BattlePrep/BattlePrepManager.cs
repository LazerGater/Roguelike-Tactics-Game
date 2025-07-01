using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattlePreparationManager : MonoBehaviour
{
    [SerializeField] private GameObject battlePrepUI;
    [SerializeField] private Transform unitListPanel; // Populate with TMP + Buttons
    [SerializeField] private GameObject unitUIPrefab;
    [SerializeField] private List<Vector2Int> startingTiles;
    [SerializeField] private Button battleButton;
    [SerializeField] private Button forfeitButton;

    private List<PlayerUnit> allUnits;
    private List<PlayerUnit> selectedUnits = new();
    private int partyLimit;

    public static event System.Action OnPartyChanged;

    private void Start()
    {
        // ----- pull map data once --------------------------------
        var ts = FindFirstObjectByType<GridInitializer>();
        startingTiles = new List<Vector2Int>(ts.AllySpawns);   
        partyLimit = ts.PartyLimit;

        // ----- build UI, preview grid, wire buttons --------------
        PopulateUnitUI();
        RefreshGridPreview();
        HighlightStartTiles(true);

        battleButton.onClick.AddListener(OnBattleStart);
        forfeitButton.onClick.AddListener(OnForfeit);
        OnPartyChanged?.Invoke();
    }

    /* =======================  UI  ============================== */

    private void PopulateUnitUI()
    {
        // clear previous children (supports scene reload)
        foreach (Transform c in unitListPanel)
            Destroy(c.gameObject);

        // build one UI row per party member
        foreach (var data in PartyCarrier.Instance.playerParty)
        {
            GameObject go = Instantiate(unitUIPrefab, unitListPanel);
            var item = go.GetComponent<UnitListItemController>();
            item.Setup(data, OnUnitToggleRequest);   // callback below
        }
    }
    private void OnUnitToggleRequest(PlayerData data, bool add)
    {
        int currentlySelected = PartyCarrier.Instance.playerParty
                                .Count(p => p.isSelectedForBattle);

        if (add && currentlySelected >= partyLimit)
        {
            Debug.LogWarning($"Party limit of {partyLimit} reached.");
            return;                           // click is ignored
        }

        // toggle the flag because the click is accepted
        data.isSelectedForBattle = add;
        OnPartyChanged?.Invoke();  // notifies UI to update

        RefreshGridPreview();
    }



    public void OnUnitPlaced(PlayerUnit unit)
    {
        if (!selectedUnits.Contains(unit)) return;
        startingTiles.Remove(unit.GetGridPosition());
        // Optional: mark as "locked in"
    }

    /* ==================  Preview & Spawn  ====================== */

    public void RefreshGridPreview()
    {
        // true  = dummy units only (no TurnManager registration)
        var gm = GridManager.Instance;
        gm.SpawnSelectedParty(startingTiles, true);
    }

    private void OnBattleStart()
    {
        // final spawn; this time units are registered for the battle
        var gm = GridManager.Instance;
        gm.SpawnSelectedParty(startingTiles, false);

        battlePrepUI.SetActive(false);
        HighlightStartTiles(false);
        TurnManager.Instance.StartBattle();
    }




    private void HighlightStartTiles(bool on)
    {
        foreach (var pos in startingTiles)
        {
            if (on) GridManager.Instance.HighlightTile(pos);
            else GridManager.Instance.ClearTileHighlight(pos);
        }
    }

    private void OnForfeit()
    {
        // TODO: implement scene reload / return to main menu
    }
}

