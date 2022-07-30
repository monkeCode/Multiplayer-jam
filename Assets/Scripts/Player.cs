using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class Player : Entity
{
    [SerializeField] private float _jumpForce;
    private Inputer _inputer;
    protected override void Start()
    {
        base.Start();
        _inputer = new Inputer();
        _inputer.Enable();
        _inputer.Player.Move.performed += ctx => UpdateMoveDirection(ctx.ReadValue<float>());
        _inputer.Player.Move.canceled += ctx => UpdateMoveDirection(0);
        _inputer.Player.Jump.performed += ctx => Jump();
    }

    private void UpdateMoveDirection(float dir)
    {
        if(photonView.IsMine)
        {
            moveDirection = dir;
        }
    }

    private void Jump()
    {
        if(photonView.IsMine)
        {
            rb.velocity = new Vector2(rb.velocity.x, _jumpForce);
        }

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            Vector3 pos = transform.localPosition;
            stream.Serialize(ref pos);
        }
        else
        {
            Vector3 pos = Vector3.zero;
            stream.Serialize(ref pos);  // pos gets filled-in. must be used somewhere
            transform.position = pos;
        }
    }
}
