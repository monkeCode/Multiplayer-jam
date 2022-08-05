using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Bullet : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private int _dmg;
    [SerializeField] private float _timeToDestroy;
    private Rigidbody2D _rigidbody2D;
    private GameObject _caster;
    [SerializeField] private float _pushForce;

    private IEnumerator Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        if (!photonView.IsMine) yield break;
        yield return new WaitForSeconds(_timeToDestroy);
        try
        {
            PhotonNetwork.Destroy(gameObject);
        }
        catch (Exception)
        {
            // ignored
        }
    }

    protected virtual void Update()
    {
        if(photonView.IsMine)
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
            stream.SendNext(_rigidbody2D.rotation);
        }
        else
        {
            Vector2 syncPos = (Vector2)stream.ReceiveNext();
            Vector2 syncVel = (Vector2)stream.ReceiveNext();
            float syncRot = (float)stream.ReceiveNext();
            try
            {
                _rigidbody2D.position = syncPos;
                _rigidbody2D.velocity = syncVel;
                _rigidbody2D.rotation = syncRot;
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!photonView.IsMine) return;
        if (_caster == col.gameObject) return;
        if (col.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(_dmg);
            if (col.TryGetComponent(out Entity rb))
            {
                rb.GetComponent<PhotonView>().RPC(nameof(rb.Push), RpcTarget.All,
                    _pushForce * _rigidbody2D.velocity.normalized);
            }
        }
        if (col.CompareTag("BulletIgnore")) return;
            PhotonNetwork.Destroy(gameObject);
    }
    
}
