using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Player : Entity
{
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _fallMultiplier = 2.5f;
    [SerializeField] private float _lowJumpMultiplier = 2f;
    [SerializeField] private float _maxSpeed;
    private bool _isJump = false;
    private Inputer _inputer;

    protected override void Start()
    {
        base.Start();
        DontDestroyOnLoad(gameObject);
        _inputer = new Inputer();
        _inputer.Enable();
        _inputer.Player.Move.performed += ctx => UpdateMoveDirection(ctx.ReadValue<float>());
        _inputer.Player.Move.canceled += ctx => UpdateMoveDirection(0);
        _inputer.Player.Jump.performed += ctx => Jump();
        _inputer.Player.Jump.canceled += ctx => _isJump = false;
        _inputer.Menu.LoadScene.performed += ctx =>
        {
            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(Server.Instance.MoveToGameScene("SampleScene"));
            }
        };
    }

    protected override void Update()
    {
        base.Update();
        if (!photonView.IsMine) return;
        if (Mathf.Abs(rb.velocity.x) > _maxSpeed)
        {
            rb.velocity = new Vector2(_maxSpeed * (rb.velocity.x>0?1:-1) , rb.velocity.y);
        }
        if(rb.velocity.y<0)
            rb.velocity += Vector2.up * (Physics2D.gravity.y * (_fallMultiplier - 1) * Time.deltaTime);
        else if(rb.velocity.y > 0 && !_isJump)
            rb.velocity += Vector2.up * (Physics2D.gravity.y * (_lowJumpMultiplier - 1) * Time.deltaTime);
    }

    private void DropItem()
    {
        
    }
    private void UpdateMoveDirection(float dir)
    {
        if (photonView.IsMine)
        {
            moveDirection = dir;
        }
    }

    private void Jump()
    {
        if (photonView.IsMine && OnGround())
        {
            _isJump = true;
            rb.velocity = Vector2.up * _jumpForce;
        }
    }
}
