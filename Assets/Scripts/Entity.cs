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

    protected virtual void Update()
    {
        Move(moveDirection);
    }

    protected virtual void Move(float xMove)
    {
        rb.velocity = new Vector2(xMove * speed, rb.velocity.y);
    }

    public virtual void TakeDamage(int damage)
    {
        Kill();
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
        }
        else
        {
            rb.velocity = (Vector2)stream.ReceiveNext();
        }
        
    }
}
