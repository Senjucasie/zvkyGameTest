using System.Collections.Generic;
using UnityEngine;

public class SymbolPool : MonoBehaviour
{
    public static SymbolPool Instance;
    internal Dictionary<int, Stack<Symbol>> poolDictionary = new();
    [SerializeField] private List<Symbol> _symbolPrefabs = new();
    [SerializeField] private int _initialSize = 10;

    private void Awake()
    {
        Instance = this;
    }

    public void CreateSymbolPool()
    {
        for (int i = 0; i < _symbolPrefabs.Count; i++)
        {
            for (int index = 0; index < _initialSize; index++)
            {
                CreateNewObject(i, _symbolPrefabs[i]);
            }
        }
    }

    private void Start()
    {
        //CreateSymbolPool();
    }

    private void CreateNewObject(int index, Symbol symbol)
    {
        Symbol newObj = Instantiate(symbol.gameObject, this.transform).GetComponent<Symbol>();
        newObj.SymbolID = index;
        newObj.gameObject.SetActive(false);
        if (!poolDictionary.ContainsKey(index))
        {
            poolDictionary[index] = new Stack<Symbol>();
        }

        poolDictionary[index].Push(newObj);
    }

    public Symbol GetObject(int index, Transform parenttransform)
    {
        if (!poolDictionary.ContainsKey(index) || poolDictionary[index].Count == 0)
        {
            CreateNewObject(index, _symbolPrefabs[index]);
        }

        Symbol symbol = poolDictionary[index].Pop();
        symbol.gameObject.transform.SetParent(parenttransform);
        symbol.gameObject.SetActive(true);
        return symbol;
    }

    public void ReturnObject(Symbol symbol)
    {
        symbol.gameObject.SetActive(false);
        symbol.gameObject.transform.SetParent(this.transform);
        poolDictionary[symbol.SymbolID].Push(symbol);
    }
}