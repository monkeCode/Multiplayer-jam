using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class WorldItem : MonoBehaviourPun, IItem, IPunObservable
{
    private  Rigidbody2D rb;
    private SpriteRenderer sr;
    private bool _canTake;
   [SerializeField] private InventoryItem _item;

   private IEnumerator Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        if (_item != null)
            sr.sprite = _item.Sprite;
        yield return new WaitForSeconds(1f);
        _canTake = true;
    }
   [PunRPC]
    public void SetItem(int itemHash)
    {
        _item = Server.Instance.GetItem(itemHash);
        if(sr != null)
            sr.sprite = _item.Sprite;
    }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //sync velocity rotation speed rotation and position
        if (stream.IsWriting)
        {
            stream.SendNext(rb.velocity);
            //stream.SendNext(rb.angularVelocity);
            stream.SendNext(transform.position);
            //stream.SendNext(transform.rotation);
        }
        else
        {
            try
            {
                rb.velocity = (Vector2) stream.ReceiveNext();
                //rb.angularVelocity = (float)stream.ReceiveNext();
                transform.position = (Vector2) stream.ReceiveNext();
                //transform.rotation = (Quaternion)stream.ReceiveNext();
            }
            catch (Exception)
            {
                // ignored
            }
        }
        
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(_canTake && col.transform.TryGetComponent(out Player player))
        {
            if (player.Item != null) return;
            player.SetItem(_item);
            PhotonNetwork.Destroy(gameObject);
            //Destroy(gameObject);
        }
    }
}
