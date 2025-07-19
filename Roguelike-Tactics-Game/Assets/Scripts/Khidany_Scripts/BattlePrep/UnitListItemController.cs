using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitListItemController : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI classText;
    public Button upButton;
    public Button downButton;

    private PartyMember memberRef;
    private System.Action<PartyMember> moveUp;
    private System.Action<PartyMember> moveDown;
    private System.Action<PartyMember> toggleSelect;

    public void Setup(PartyMember member, System.Action<PartyMember> onUp, System.Action<PartyMember> onDown, System.Action<PartyMember> onToggleSelect)
    {
        memberRef = member;
        moveUp = onUp;
        moveDown = onDown;
        toggleSelect = onToggleSelect;

        nameText.text = member.character.characterName;
        classText.text = member.unitClass.className;


        upButton.onClick.AddListener(() => moveUp(memberRef));
        downButton.onClick.AddListener(() => moveDown(memberRef));
    }
}
