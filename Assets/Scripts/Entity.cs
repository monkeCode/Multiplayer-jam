using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(SpriteRenderer))]
public class Entity : MonoBehaviour, IDamageable, IPunObservable
{
    [SerializeField]protected float speed;
    [SerializeField] private LayerMask _groundLayer;
    protected Rigidbody2D rb;
    protected float moveDirection;
    protected PhotonView photonView;
    protected SpriteRenderer spriteRenderer;
    protected Collider2D collider2D;
    protected bool isGrounded { get; private set; }
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        photonView = GetComponent<PhotonView>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider2D = GetComponent<Collider2D>();
    }

    protected virtual void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            Move(moveDirection);
        }
    }

    protected virtual void Update()
    {
        isGrounded = OnGround();
    }

    protected virtual void Move(float xMove)
    {
        rb.AddForce(new Vector2(xMove * speed, 0));
    }

    public virtual void TakeDamage(int damage)
    {
        Kill();
    }

   protected bool OnGround()
   {
       var point = collider2D.bounds.center - new Vector3(0, collider2D.bounds.extents.y, 0);
       var hits = Physics2D.RaycastAll(point, Vector2.down, 0.1f, _groundLayer);
       var index= Array.FindIndex(hits, hit2D => hit2D.collider?.gameObject != gameObject);
       Debug.DrawRay(point, Vector2.down * 0.1f, Color.red);
       return index != -1;
   }
    public virtual void Heal(int heal)
    {
        
    }

    public virtual void Kill()
    {
        Destroy(gameObject);
    }

    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(rb.velocity);
            stream.SendNext(transform.position);
        }
        else
        {
            try
            {
                rb.velocity = (Vector2)stream.ReceiveNext();
                transform.position = (Vector3)stream.ReceiveNext();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
           
        }
        
    }
}
