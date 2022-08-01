using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Bullet : MonoBehaviourPun, IPunObservable
{
    private Rigidbody2D _rigidbody2D;
    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
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
}
