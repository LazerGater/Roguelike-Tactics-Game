using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PhaseUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI phaseText;

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

        TurnPhase phase = TurnManager.Instance.CurrentPhase;

        phaseText.text = $"Phase: {phase}";
    }
}
