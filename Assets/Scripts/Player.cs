using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Entity
{
    enum PlayerState
    {
        Idle, Run, Jump, Fall, Hit
    }
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _fallMultiplier = 2.5f;
    [SerializeField] private float _lowJumpMultiplier = 2f;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private InventoryItem _item;
    [SerializeField] private LayerMask _wallLayer;
    [SerializeField] private PlayerState _playerState { 
        get => (PlayerState) _animator.GetInteger("State"); 
        set => _animator.SetInteger("State", (int)value); }
    public InventoryItem Item => _item;
    private bool _isJump;
    private Inputer _inputer;
    private Animator _animator;
    protected override void Start()
    {
        base.Start();
        _animator = GetComponent<Animator>();
        _playerState = _playerState;
        DontDestroyOnLoad(gameObject);
        _inputer = new Inputer();
        _inputer.Enable();
        _inputer.Player.Move.performed += ctx => UpdateMoveDirection(ctx.ReadValue<float>());
        _inputer.Player.Move.canceled += ctx => UpdateMoveDirection(0);
        _inputer.Player.Jump.performed += ctx => Jump();
        _inputer.Player.Jump.canceled += ctx => _isJump = false;
        _inputer.Player.Drop.performed += context =>
        {
            if (_item != null && photonView.IsMine)
            {
                var force = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()) - transform.position;
                //photonView.RPC(nameof(DropItem), RpcTarget.All, (Vector2)force);
                DropItem(force.normalized);
            }
        };
        _inputer.Player.ItemShoot.performed += context =>
        {
            if (_item != null && photonView.IsMine)
                _item.Use(this);
        };
        _inputer.Player.ItemShoot.canceled += context =>
        {
            if (_item != null && photonView.IsMine)
                _item.CanselUse();
        };
        _inputer.Menu.LoadScene.performed += ctx =>
        {
            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(Server.Instance.MoveToGameScene("SampleScene"));
            }
        };
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
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

    protected override void Update()
    {
        base.Update();
        
            if (rb.velocity.y < 0.1f)
                _playerState = PlayerState.Fall;
        if (_playerState == PlayerState.Fall && isGrounded)
        {
            _playerState = moveDirection ==0 ? PlayerState.Idle : PlayerState.Run;
        }
        spriteRenderer.flipX = rb.velocity.x < 0.2f;

    }

    [PunRPC]
    private void DropItem(Vector2 force)
    {
       var item = PhotonNetwork.Instantiate("WorldItemPrefab", transform.position, Quaternion.identity);
       item.GetComponent<PhotonView>().RPC("SetItem", RpcTarget.All, _item.ItemName);
       item.GetComponent<Rigidbody2D>().velocity = force.normalized * 13;
       item.GetComponent<Rigidbody2D>().angularVelocity = 20;
       photonView.RPC(nameof(DeleteActiveItem), RpcTarget.All);
    }

    [PunRPC]
    public void DeleteActiveItem()
    {
        _item = null;
    }
    [PunRPC]
    public void SetItem(string item)
    {
        if (_item != null) return;
        Debug.Log($"{photonView.Owner.UserId} set item {item}");
        
        _item = Server.Instance.GetItem(item);
    }
    private void UpdateMoveDirection(float dir)
    {
        if (photonView.IsMine)
        {
            moveDirection = dir;
            if(_playerState == PlayerState.Idle && dir != 0)
                _playerState = PlayerState.Run;
            else if(_playerState == PlayerState.Run && dir == 0)
                _playerState = PlayerState.Idle;
        }
    }

    private void Jump()
    {
        if (photonView.IsMine)
        {
            var wall = OnWall();
            if (!isGrounded && wall == 0 ) return;
            _isJump = true;
            rb.velocity = Vector2.up * _jumpForce;
            rb.velocity += Vector2.right * -wall * _jumpForce;
            _playerState = PlayerState.Jump;
        }
    }

    public override void TakeDamage(int damage)
    {
        if(_currentHealth <= 0) return;
        photonView.RPC(nameof(TakeDamageAnim), RpcTarget.All);
        base.TakeDamage(damage);
    }

    [PunRPC]
    private void TakeDamageAnim()
    {
        _animator.SetTrigger("Damage");
    }
    private int OnWall()
    {
        if(isGrounded) return 0;
        //cast two ray from collider bounds to detect wall
        var colliderBounds = collider2D.bounds;
        var leftRay = new Ray(colliderBounds.center - new Vector3(colliderBounds.extents.x, 0, 0), Vector3.left);
        var rightRay = new Ray(colliderBounds.center + new Vector3(colliderBounds.extents.x, 0, 0), Vector3.right);
        var leftHit = Physics2D.Raycast(leftRay.origin, leftRay.direction,  0.1f, _wallLayer);
        var rightHit = Physics2D.Raycast(rightRay.origin, rightRay.direction, 0.1f, _wallLayer);
        //draw debug rays
        Debug.DrawRay(leftRay.origin, leftRay.direction * (colliderBounds.extents.x + 0.1f), Color.red);
        Debug.DrawRay(rightRay.origin, rightRay.direction * (colliderBounds.extents.x + 0.1f), Color.red);
        //return left or right wall
        if (leftHit.collider != null)
            return -1;
        return rightHit.collider!=null ? 1 : 0;
    }
    public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        base.OnPhotonSerializeView(stream, info);
        //sync state
        if (stream.IsWriting)
        {
            stream.SendNext((int)_playerState);
        }
        else
        {
            try
            {
                _playerState = (PlayerState)stream.ReceiveNext();
            }
            catch (Exception e)
            {
                //Debug.LogError(e);
            }
            
        }
    }
}
