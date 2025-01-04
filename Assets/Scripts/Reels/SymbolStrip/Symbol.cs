using UnityEngine;

public class Symbol : MonoBehaviour
{
    [SerializeField] public GameObject spriteNode;
    [SerializeField] public GameObject spineNode;
    public AudioClip _audioClip;
    private int _symbolID;
    [HideInInspector] public bool isSpecialSymbol;

    [SerializeField] private PaylinesAnimationController paylineAnimationController;

    public int SymbolID { get => _symbolID; set => _symbolID = value; }

    private void Start()
    {
        HideWin();
    }

    public void ShowWin(PaylineController.PayLineState paylinestate)
    {
        spriteNode.SetActive(false);
        spineNode.gameObject.SetActive(true);

        if (paylineAnimationController != null)
        {
            paylineAnimationController.ManagePaylineAnimForGameState(paylinestate);
        }
    }

   

    public void HideWin()
    {
        spriteNode.SetActive(true);
        spineNode.gameObject.SetActive(false);
    }

    public void SetSpineSortingOrder(int setTo)
    {
        MeshRenderer mr = spineNode.GetComponent<MeshRenderer>();
        mr.sortingOrder = setTo;
    }
}
