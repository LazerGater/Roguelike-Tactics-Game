using System;                    // <-- new
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitListItemController : MonoBehaviour
{
    [SerializeField] private Image portraitImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private Button toggleButton;

    private PlayerData data;

    private Action<PlayerData, bool> onToggleRequest;

    public void Setup(PlayerData unitData)
    {
        InternalSetup(unitData);
    }


    public void Setup(PlayerData unitData, Action<PlayerData, bool> toggleCallback)
    {
        onToggleRequest = toggleCallback;
        InternalSetup(unitData);
    }


    private void InternalSetup(PlayerData unitData)
    {
        data = unitData;

        portraitImage.sprite = data.portrait;
        nameText.text = data.unitName;
        hpText.text = $"HP: {data.currentHP}/{data.maxHP}";

        UpdateVisual();

        toggleButton.onClick.RemoveAllListeners();
        toggleButton.onClick.AddListener(ToggleSelected);
    }

    private void ToggleSelected()
    {
        bool add = !data.isSelectedForBattle;

        if (onToggleRequest != null)
        {
            onToggleRequest.Invoke(data, add);      // manager decides
        }
        else
        {
            data.isSelectedForBattle = add;         // fallback
            FindFirstObjectByType<BattlePreparationManager>()?.RefreshGridPreview();
        }

        UpdateVisual();                             // reflect final flag
    }


    private void UpdateVisual()
    {
        // Gray out when not selected
        Color tint = data.isSelectedForBattle ? Color.white : new Color(0.7f, 0.7f, 0.7f);
        portraitImage.color = tint;
        nameText.color = tint;
        hpText.color = tint;
    }
}
