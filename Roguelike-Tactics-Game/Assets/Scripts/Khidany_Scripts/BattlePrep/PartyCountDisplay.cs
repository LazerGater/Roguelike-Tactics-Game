using TMPro;
using UnityEngine;

public class PartyCountDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countText;

    private int partyLimit;

    private void Start()
    {
        var ts = FindFirstObjectByType<GridInitializer>();
        partyLimit = ts.PartyLimit;

        UpdateDisplay();
    }

    private void OnEnable()
    {
        BattlePreparationManager.OnPartyChanged += UpdateDisplay;
    }

    private void OnDisable()
    {
        BattlePreparationManager.OnPartyChanged -= UpdateDisplay;
    }

    private void UpdateDisplay()
    {
        int selected = PartyCarrier.Instance.playerParty
                        .FindAll(p => p.isSelectedForBattle).Count;

        if (partyLimit == 0)
        {
            partyLimit = FindFirstObjectByType<GridInitializer>()?.PartyLimit ?? 0;
        }
        countText.text = $"Party: {selected} / {partyLimit}";
    }
}
