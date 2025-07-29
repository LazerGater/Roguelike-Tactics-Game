using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitStatsUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI atkText;
    public TextMeshProUGUI defText;
    public TextMeshProUGUI spdText;
    public TextMeshProUGUI dexText;
    public TextMeshProUGUI lukText;
    public Image portraitImage;

    public GameObject rootObject; // Optional: for enabling/disabling the full UI

    public void Show(UnitStats stats)
    {
        if (rootObject != null)
            rootObject.SetActive(true);

        hpText.text = $"HP: {stats.currentHP}/{stats.maxHP}";
        atkText.text = $"Attack: {stats.atk}";
        defText.text = $"Defence: {stats.def}";
        spdText.text = $"Speed: {stats.speed}";
        dexText.text = $"Dexterity: {stats.dex}";
        lukText.text = $"Luck: {stats.luck}";
    }

    public void Hide()
    {
        if (rootObject != null)
            rootObject.SetActive(false);
    }
}
