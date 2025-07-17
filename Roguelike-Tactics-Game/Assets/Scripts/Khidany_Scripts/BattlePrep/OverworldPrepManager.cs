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
    public UnityEngine.UI.Button loadButton;
    public UnityEngine.UI.Button backButton;

    private void Start()
    {
        PopulateUnitList();
        loadButton.onClick.AddListener(LoadBattleScene);
        backButton.onClick.AddListener(HandleBack);
    }

    private void PopulateUnitList()
    {
        // Sort by priorityID ascending
        var list = PartyCarrier.Instance.playerParty;
        list.Sort((a, b) => a.priorityID.CompareTo(b.priorityID));

        // Clear old items
        foreach (Transform child in unitListPanel)
            Destroy(child.gameObject);

        // Rebuild
        foreach (var data in list)
        {
            var go = Instantiate(unitUIPrefab, unitListPanel);
            var item = go.GetComponent<UnitListItemController>();
            item.Setup(data, OnMoveUp, OnMoveDown);
        }
    }

    private void OnMoveUp(PlayerData data)
    {
        var list = PartyCarrier.Instance.playerParty;
        int idx = list.IndexOf(data);
        if (idx > 0)
        {
            int tmp = list[idx - 1].priorityID;
            list[idx - 1].priorityID = data.priorityID;
            data.priorityID = tmp;
            PopulateUnitList();
        }
    }

    private void OnMoveDown(PlayerData data)
    {
        var list = PartyCarrier.Instance.playerParty;
        int idx = list.IndexOf(data);
        if (idx < list.Count - 1)
        {
            int tmp = list[idx + 1].priorityID;
            list[idx + 1].priorityID = data.priorityID;
            data.priorityID = tmp;
            PopulateUnitList();
        }
    }

    private void LoadBattleScene()
    {
        SceneManager.LoadScene("BattleScene");
    }

    private void HandleBack()
    {
        // implement as needed (cancel prep)
    }
}