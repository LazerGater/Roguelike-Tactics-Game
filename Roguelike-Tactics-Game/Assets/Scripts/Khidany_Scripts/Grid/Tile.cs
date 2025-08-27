using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private GameObject highlight;

    public void SetHighlight(bool value)
    {
        highlight.SetActive(value);
    }
    private void OnMouseEnter()
    {
        highlight.SetActive(true);
    }

    private void OnMouseExit()
    {
        highlight.SetActive(false);
    }
}
