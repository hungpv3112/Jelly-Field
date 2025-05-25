using System;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private ItemData itemData;
}

[Serializable]
public class ItemData
{
    public List<int> itemIds;

}