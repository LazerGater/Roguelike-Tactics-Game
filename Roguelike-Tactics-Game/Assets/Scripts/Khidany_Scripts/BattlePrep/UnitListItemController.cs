using System;                    // <-- new
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitListItemController : MonoBehaviour
{
    public Image portraitImage;
    public TextMeshProUGUI nameText;
    public Button upButton;
    public Button downButton;

    private PlayerData data;
    private Action<PlayerData> onUp;
    private Action<PlayerData> onDown;

    public void Setup(
        PlayerData unitData,
        Action<PlayerData> moveUpCallback,
        Action<PlayerData> moveDownCallback
    )
    {
        data = unitData;
        onUp = moveUpCallback;
        onDown = moveDownCallback;

        portraitImage.sprite = data.portrait;
        nameText.text = data.unitName;

        upButton.onClick.RemoveAllListeners();
        upButton.onClick.AddListener(() => onUp(data));

        downButton.onClick.RemoveAllListeners();
        downButton.onClick.AddListener(() => onDown(data));

        // disable at ends
        upButton.interactable = data.priorityID > 0;
        downButton.interactable = true; // optional: disable if at bottom
    }
}