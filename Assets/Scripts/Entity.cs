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
    protected Rigidbody2D rb;
    protected float moveDirection;
    protected PhotonView photonView;
    protected SpriteRenderer spriteRenderer;
    
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        photonView = GetComponent<PhotonView>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected virtual void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            Move(moveDirection);
        }
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
       return true;
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
