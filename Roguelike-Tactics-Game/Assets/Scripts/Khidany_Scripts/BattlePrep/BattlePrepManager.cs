using System.Collections.Generic;
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

    public void Start()
    {
        allUnits = new List<PlayerUnit>(); // Clear - we use data now
        selectedUnits.Clear();

        PopulateUnitUI();
        HighlightStartTiles(true);

        battleButton.onClick.AddListener(OnBattleStart);
        forfeitButton.onClick.AddListener(OnForfeit);
    }



    private void PopulateUnitUI()
    {
        foreach (Transform child in unitListPanel)
            Destroy(child.gameObject);

        foreach (var data in PartyCarrier.Instance.playerParty)
        {
            GameObject go = Instantiate(unitUIPrefab, unitListPanel);
            var controller = go.GetComponent<UnitListItemController>();
            controller.Setup(data);
        }
    }
    public void RefreshGridPreview()
    {
        // preview = true
        GridManager.Instance.SpawnSelectedParty(startingTiles, true);
    }



    public void OnUnitPlaced(PlayerUnit unit)
    {
        if (!selectedUnits.Contains(unit)) return;
        startingTiles.Remove(unit.GetGridPosition());
        // Optional: mark as "locked in"
    }

    private void HighlightStartTiles(bool on)
    {
        foreach (var pos in startingTiles)
        {
            if (on)
                GridManager.Instance.HighlightTile(pos); // Add method
            else
                GridManager.Instance.ClearTileHighlight(pos);
        }
    }

    private void OnBattleStart()
    {
        // Actually spawn everyone now
        GridManager.Instance.SpawnSelectedParty(startingTiles);

        battlePrepUI.SetActive(false);
        HighlightStartTiles(false);
        TurnManager.Instance.StartBattle();
    }



    private void OnForfeit()
    {
        // Scene reload or main menu
    }
}

