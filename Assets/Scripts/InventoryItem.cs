using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/TestItem")]
public class InventoryItem : ScriptableObject
{
    [SerializeField] private string _itemName;
    [SerializeField] private Sprite _sprite;
    public Sprite Sprite => _sprite;
    public string ItemName => _itemName;

    public virtual void Use()
    {
        Debug.Log("Using " + _itemName);
    }
    
}
