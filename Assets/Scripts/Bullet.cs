using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Bullet : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private int _dmg;
    private Rigidbody2D _rigidbody2D;
    private GameObject _caster;
    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        
    }

    private void Update()
    {
       _rigidbody2D.rotation = Mathf.Atan2(_rigidbody2D.velocity.y, _rigidbody2D.velocity.x) * Mathf.Rad2Deg - 90;
    }

    public void SetCaster(GameObject caster)
    {
        _caster = caster;
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //sync transform and velocity
        if (stream.IsWriting)
        {
            stream.SendNext(_rigidbody2D.position);
            stream.SendNext(_rigidbody2D.velocity);
        }
        else
        {
            Vector2 syncPos = (Vector2)stream.ReceiveNext();
            Vector2 syncVel = (Vector2)stream.ReceiveNext();
            _rigidbody2D.position = syncPos;
            _rigidbody2D.velocity = syncVel;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!photonView.IsMine) return;
        if(_caster == col.gameObject) return;
        if (col.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(_dmg);
        }
        PhotonNetwork.Destroy(gameObject);
    }
    
}
