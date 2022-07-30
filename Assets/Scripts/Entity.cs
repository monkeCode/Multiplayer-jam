using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class Entity : MonoBehaviour, IDamageable
{
    [SerializeField]protected float speed;
    protected Rigidbody2D rb;
    protected float moveDirection;
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
}
