using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Player : Entity, IPunObservable
{
    [SerializeField] private float _jumpForce;
    private Inputer _inputer;
    private PhotonView _photonView;
    private SpriteRenderer _renderer;
    protected override void Start()
    {
        base.Start();
        _inputer = new Inputer();
        _inputer.Enable();
        _inputer.Player.Move.performed += ctx => UpdateMoveDirection(ctx.ReadValue<float>());
        _inputer.Player.Move.canceled += ctx => UpdateMoveDirection(0);
        _inputer.Player.Jump.performed += ctx => Jump();
        _renderer = GetComponent<SpriteRenderer>();
        _renderer.color = Color.green;
        _photonView = GetComponent<PhotonView>();
    }

    private void UpdateMoveDirection(float dir)
    {
        if(_photonView.IsMine)
        {
            moveDirection = dir;
        }
    }

    private void Jump()
    {
        if(_photonView.IsMine)
        {
            rb.velocity = new Vector2(rb.velocity.x, _jumpForce);
        }

    }
    
    [PunRPC]
    void GetPing()
    { 
        Debug.Log(PhotonNetwork.GetPing());
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
