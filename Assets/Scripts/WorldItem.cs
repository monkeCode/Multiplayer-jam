using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class WorldItem : MonoBehaviourPun, IItem, IPunObservable
{
    private  Rigidbody2D rb;
    private SpriteRenderer sr;
   [SerializeField] private InventoryItem _item;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }
    
    
    [PunRPC]
    public InventoryItem Take()
    {
        return _item;
        PhotonNetwork.Destroy(gameObject);
    }
    [PunRPC]
    public void SetItem(InventoryItem item)
    {
        
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //sync velocity rotation speed rotation and position
        if (stream.IsWriting)
        {
            stream.SendNext(rb.velocity);
            stream.SendNext(rb.angularVelocity);
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            rb.velocity = (Vector2)stream.ReceiveNext();
            rb.angularVelocity = (float)stream.ReceiveNext();
            transform.position = (Vector2)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
        }
        
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        
    }
}
