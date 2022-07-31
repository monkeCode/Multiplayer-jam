using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/TestItem")]
public class InventoryItem : ScriptableObject
{
    [SerializeField] private string _itemName;
    [SerializeField] private Sprite _sprite;
    public Sprite Sprite => _sprite;
    public string ItemName => _itemName;
    [PunRPC]
    public virtual void Use()
    {
        Debug.Log("Using " + _itemName);
    }
    [PunRPC]
    public virtual void CanselUse()
    {
        Debug.Log("Cansel using " + _itemName);
    }
    
}
