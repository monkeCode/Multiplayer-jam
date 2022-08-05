using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class WorldItem : MonoBehaviourPun, IItem, IPunObservable
{
    
    private  Rigidbody2D rb;
    private SpriteRenderer sr;
    private bool _canTake;
    private AudioSource _audioSource;
   [SerializeField] private InventoryItem _item;
   [SerializeField] private AudioClip _pickupSound;
    [SerializeField] private AudioClip _dropSound;
    private IEnumerator Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        _audioSource = GetComponent<AudioSource>();
        if (_item != null)
            sr.sprite = _item.Sprite;
        yield return new WaitForSeconds(0.5f);
        _canTake = true;
    }
   [PunRPC]
    public void SetItem(string itemName)
    {
        _item = Server.Instance.GetItem(itemName);
        if(sr != null)
            sr.sprite = _item.Sprite;
        _audioSource.clip = _dropSound;
        _audioSource.Play();
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

    [PunRPC]
    private void PickUpSound()
    {
        _audioSource.clip = _pickupSound;
        _audioSource.Play();
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if(_canTake && photonView.IsMine && col.transform.TryGetComponent(out Player player))
        {
            if (player.Item != null) return;
            player.GetComponent<PhotonView>().RPC(nameof(Player.SetItem), RpcTarget.All, _item.ItemName);
            photonView.RPC(nameof(PickUpSound), RpcTarget.All);
            PhotonNetwork.Destroy(gameObject);
            //Destroy(gameObject);
        }
    }
    
}
