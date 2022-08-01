using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Entity
{
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _fallMultiplier = 2.5f;
    [SerializeField] private float _lowJumpMultiplier = 2f;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private InventoryItem _item;
    public InventoryItem Item => _item;
    private bool _isJump;
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

    [PunRPC]
    private void DropItem(Vector2 force)
    {
       var item = PhotonNetwork.Instantiate("WorldItemPrefab", transform.position, Quaternion.identity);
       item.GetComponent<PhotonView>().RPC("SetItem", RpcTarget.All, _item.ItemName);
       item.GetComponent<Rigidbody2D>().velocity = force * 2;
       item.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(-100, 100);
       photonView.RPC(nameof(DeleteActiveItem), RpcTarget.All);
    }

    [PunRPC]
    private void DeleteActiveItem()
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
