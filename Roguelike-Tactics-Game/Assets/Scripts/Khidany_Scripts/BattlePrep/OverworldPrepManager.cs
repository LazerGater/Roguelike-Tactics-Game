using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OverworldPrepManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform unitListPanel;
    public GameObject unitUIPrefab;
    public Button loadButton;
    public Button backButton;

    private void Start()
    {
        PopulateUnitList();
        loadButton.onClick.AddListener(LoadBattleScene);
        backButton.onClick.AddListener(HandleBack);
    }

    private void PopulateUnitList()
    {
        var list = PartyCarrier.Instance.playerParty;
        list.Sort((a, b) => a.priorityID.CompareTo(b.priorityID));

        foreach (Transform child in unitListPanel)
            Destroy(child.gameObject);

        foreach (var member in list)
        {
            var go = Instantiate(unitUIPrefab, unitListPanel);
            var item = go.GetComponent<UnitListItemController>();
            item.Setup(member, OnMoveUp, OnMoveDown, OnToggleSelect);
        }
    }

    private void OnMoveUp(PartyMember member)
    {
        var list = PartyCarrier.Instance.playerParty;
        int idx = list.IndexOf(member);
        if (idx > 0)
        {
            int tmp = list[idx - 1].priorityID;
            list[idx - 1].priorityID = member.priorityID;
            member.priorityID = tmp;
            PopulateUnitList();
        }
    }

    private void OnMoveDown(PartyMember member)
    {
        var list = PartyCarrier.Instance.playerParty;
        int idx = list.IndexOf(member);
        if (idx < list.Count - 1)
        {
            int tmp = list[idx + 1].priorityID;
            list[idx + 1].priorityID = member.priorityID;
            member.priorityID = tmp;
            PopulateUnitList();
        }
    }

    private void OnToggleSelect(PartyMember member)
    {
        member.isSelectedForBattle = !member.isSelectedForBattle;
        PopulateUnitList();
    }

    private void LoadBattleScene()
    {
        SceneManager.LoadScene("BattleScene");
    }

    private void HandleBack()
    {
        // implement as needed
    }
}
