using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class Saw : MonoBehaviourPun, IToggle
{
    [SerializeField] private float _speed;
    [SerializeField] private bool _isOn;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private int _damage;
    private Rigidbody2D _rigidbody2D;
    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();

    }
    
    void Update()
    {
        if (photonView.IsMine)
        {
            if(_isOn)
                _rigidbody2D.velocity = GetMoveDir();
            else
            {
                _rigidbody2D.velocity += Vector2.up * Physics2D.gravity * Time.deltaTime;
            }
        }
        
    }

    public void Toggle(bool isOn)
    {
        _isOn = isOn;
    }

    private Vector2 GetMoveDir()
    {
        //get normals by raycasting
        Vector2[] normals = new Vector2[4];
        RaycastHit2D[] hits = new RaycastHit2D[4];
        Vector2[] directions = new Vector2[4];
        directions[0] = new Vector2(0, 1);
        directions[1] = new Vector2(1, 0);
        directions[2] = new Vector2(0, -1);
        directions[3] = new Vector2(-1, 0);
        for (int i = 0; i < 4; i++)
        {
            hits[i] = Physics2D.Raycast(transform.position, directions[i], 0.2f, _groundLayer);
            //debug ray
            Debug.DrawRay(transform.position, directions[i] * 0.2f, Color.red);
            if (hits[i].collider != null)
            {
                normals[i] = hits[i].normal;
            }
            else
            {
                normals[i] = Vector2.zero;
            }
        }
        //get the direction by the normal
        Vector2 moveDir = Vector2.zero;
        for (int i = 0; i < 4; i++)
        {
            if (normals[i] != Vector2.zero)
            {
                moveDir += normals[i].Perpendicular1();
            }
        }

        if (moveDir == Vector2.zero)
            return _rigidbody2D.velocity + Physics2D.gravity * Time.deltaTime + Vector2.right * (_speed * Time.deltaTime);
        return moveDir * _speed;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (photonView.IsMine && col.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(_damage);
        }
    }
}
