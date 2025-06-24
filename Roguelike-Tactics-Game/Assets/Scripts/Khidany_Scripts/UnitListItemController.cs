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

    public void Setup(PlayerData unitData)
    {
        data = unitData;
        portraitImage.sprite = data.portrait;
        nameText.text = data.unitName;
        hpText.text = "HP: " + data.currentHP + "/" + data.maxHP;

        UpdateVisual();

        toggleButton.onClick.AddListener(ToggleSelected);
    }

    private void ToggleSelected()
    {
        data.isSelectedForBattle = !data.isSelectedForBattle;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        // Example: color gray when not selected
        Color color = data.isSelectedForBattle ? Color.white : new Color(0.7f, 0.7f, 0.7f);
        portraitImage.color = color;
        nameText.color = color;
        hpText.color = color;
    }
}
