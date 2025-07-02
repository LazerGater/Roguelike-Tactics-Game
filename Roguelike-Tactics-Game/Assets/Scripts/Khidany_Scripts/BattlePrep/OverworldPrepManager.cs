using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class OverworldPrepManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform unitListPanel;
    public GameObject unitUIPrefab;
    public Button loadButton;
    public Button backButton;
    public TextMeshProUGUI partyCountText;

    [Header("Grid Preview")]
    public GameObject gridManagerObject; // assign your GridManager in inspector

    private List<Vector2Int> startingTiles;
    private int partyLimit;

    public static event Action OnPartyChanged;

    private void Start()
    {
        // 1) Pull map data
        var initializer = FindObjectOfType<GridInitializer>();
        startingTiles = new List<Vector2Int>(initializer.AllySpawns);
        partyLimit = initializer.PartyLimit;

        // 2) Build unit list UI
        PopulateUnitUI();

        // 3) Preview spawn tiles
        RefreshGridPreview();
        HighlightStartTiles(true);

        // 4) Wire buttons
        loadButton.onClick.AddListener(LoadBattleScene);
        backButton.onClick.AddListener(HandleBack);

        // 5) Initialize count display
        OnPartyChanged?.Invoke();
    }

    private void PopulateUnitUI()
    {
        // sort by priorityID
        PartyCarrier.Instance.playerParty
            .Sort((a, b) => a.priorityID.CompareTo(b.priorityID));

        // clear old rows
        foreach (Transform child in unitListPanel)
            Destroy(child.gameObject);

        foreach (var data in PartyCarrier.Instance.playerParty)
        {
            //var go = Instantiate(unitUIPrefab, unitListPanel);
            //var item = go.GetComponent<UnitListItemController>();
            //item.Setup(
            //    data,
            //    OnToggleRequest,
            //    OnMoveUpRequest,
            //    OnMoveDownRequest
            //);
        }
    }

    private void OnToggleRequest(PlayerData data, bool add)
    {
        int selectedCount = PartyCarrier.Instance.playerParty
                              .Count(p => p.isSelectedForBattle);
        if (add && selectedCount >= partyLimit)
            return; // ignore beyond limit

        data.isSelectedForBattle = add;
        OnPartyChanged?.Invoke();
        RefreshGridPreview();
    }

    private void OnMoveUpRequest(PlayerData data)
    {
        var list = PartyCarrier.Instance.playerParty;
        int idx = list.IndexOf(data);
        if (idx > 0)
        {
            // swap priorityIDs
            int tmp = list[idx - 1].priorityID;
            list[idx - 1].priorityID = data.priorityID;
            data.priorityID = tmp;
            OnPartyChanged?.Invoke();
            PopulateUnitUI();
            RefreshGridPreview();
        }
    }

    private void OnMoveDownRequest(PlayerData data)
    {
        var list = PartyCarrier.Instance.playerParty;
        int idx = list.IndexOf(data);
        if (idx >= 0 && idx < list.Count - 1)
        {
            int tmp = list[idx + 1].priorityID;
            list[idx + 1].priorityID = data.priorityID;
            data.priorityID = tmp;
            OnPartyChanged?.Invoke();
            PopulateUnitUI();
            RefreshGridPreview();
        }
    }

    public void RefreshGridPreview()
    {
        var gm = gridManagerObject.GetComponent<GridManager>();
        gm.SpawnSelectedParty(startingTiles, true);
    }

    private void HighlightStartTiles(bool on)
    {
        var gm = gridManagerObject.GetComponent<GridManager>();
        foreach (var pos in startingTiles)
        {
            if (on) gm.HighlightTile(pos);
            else gm.ClearTileHighlight(pos);
        }
    }

    private void LoadBattleScene()
    {
        SceneManager.LoadScene("BattleScene");
    }

    private void HandleBack()
    {
        // close any subpanel or exit prep
        // implement as you need (e.g. hide panel or return to menu)
    }
}