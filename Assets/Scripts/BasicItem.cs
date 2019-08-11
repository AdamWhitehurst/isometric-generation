using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Item;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Collider))]
public class BasicItem : MonoBehaviour, IInventoryItem
{
    #region Fields 
    [SerializeField] public ItemData data { get; private set; }
    private MeshFilter filter;

    #endregion

    #region MonoBehaviour Methods
    void Awake()
    {
        ItemData data = new ItemData();

        data.name = "UNSET ITEM";
        data.sizeInBulk = 1;
        data.spriteId = 0;
        data.type = ItemType.Basic;
        this.name = "UNSET ITEM";
    }

    void Start()
    {
        filter = GetComponent<MeshFilter>();
        SetSprite();
    }

    void LateUpdate()
    {
        transform.forward = Camera.main.transform.forward;
    }

    #endregion

    #region IInventoryItem Methods
    public void AddToInventory(InventoryController invRef)
    {
        if (invRef.AddItem(data))
        {
            Destroy(this.gameObject);
        }
    }

    public Sprite GetSprite()
    {
        return Loader.BasicSprite(data.spriteId);
    }

    #endregion


    #region BasicItem Methods
    public void SetItem(ItemData data)
    {
        this.data = data;
    }

    void SetSprite()
    {
        filter.mesh.uv = Loader.BasicSprite(data.spriteId).uv;
    }


    #endregion

}