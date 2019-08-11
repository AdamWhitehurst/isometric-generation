using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Item;
public class InventoryController : MonoBehaviour
{
    public int bulkCap { get; private set; }
    public int currentBulkSize { get; private set; }
    public List<Item.ItemData> items;

    void Start()
    {
        items = new List<Item.ItemData>();
        if (bulkCap == 0) bulkCap = 10;
    }

    public bool AddItem(ItemData data)
    {
        if (currentBulkSize + data.sizeInBulk <= bulkCap)
        {
            items.Add(data);
            return true;
        }
        else return false;
    }

    // public List<BlockItem> BlockItems
    // {
    //     get
    //     {
    //         return items.Where(itm => itm is BlockItem).Select(itm => itm as BlockItem).ToList();
    //     }
    // }

}