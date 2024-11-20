using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="ItemPicker",menuName ="Item Picker")]
public class ItemPicker : ScriptableObject
{
    public static ItemPicker _main;
    public static ItemPicker main
    {
        get
        {
            if (_main == null)
                _main = Resources.Load<ItemPicker>("ItemPicker");
            return _main;
        }
        set { _main = value; }
    }

    public List<ItemWeight> itemList;

    public static GameObject Get()
    {
        List<int> weights = new();
        for (int i = 0; i < main.itemList.Count; i++)
        {
            weights.Add(main.itemList[i].weight);
        }
        return main.itemList[Utility.WeightedRandom(weights.ToArray())].item;
    }

    [Serializable]
    public struct ItemWeight
    {
        public GameObject item;
        public int weight;
    }
}
