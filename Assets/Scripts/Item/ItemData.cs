using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Item
{
    [System.Serializable]

    [CreateAssetMenu(fileName = "New ItemData", menuName = "ItemData", order = 1)]
    public class ItemData : ScriptableObject
    {
        public ItemType type;
        public new string name;
        public int sizeInBulk;
        public int spriteId;
    }
}