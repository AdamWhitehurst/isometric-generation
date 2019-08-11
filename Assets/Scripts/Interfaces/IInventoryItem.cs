using UnityEngine;
using Item;
public interface IInventoryItem
{
    ItemData data { get; }
    void AddToInventory(InventoryController invRef);
    Sprite GetSprite();
}