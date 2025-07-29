using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurnUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI turnText;

    private void Start()
    {
        UpdateText(); // Initialize
    }

    private void Update()
    {
        UpdateText(); // Update every frame (can optimize later with events)
    }

    private void UpdateText()
    {
        if (TurnManager.Instance == null) return;

        int turn = TurnManager.Instance.CurrentTurn;

        turnText.text = $"Turn: {turn}\n";
    }
}
