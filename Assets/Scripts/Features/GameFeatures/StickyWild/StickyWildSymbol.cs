using UnityEngine;

public class StickyWildSymbol : MonoBehaviour
{
    [SerializeField] private GameObject spineNode;

    private void Awake()
    {
        Hide();
    }

    public void Show()
    {
        spineNode.SetActive(true);
    }

    public void Hide()
    {
        spineNode.gameObject.SetActive(false);
    }
}
