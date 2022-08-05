using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class Mushroom : Entity
{
    protected override void Start()
    {
        base.Start();
        moveDirection = 1;
    }

    protected override void Update()
    {
        if(photonView.IsMine)
            CheckMoveDir();
        spriteRenderer.flipX = rb.velocity.x > 0;
    }

    protected override void Move(float xMove)
    {
        rb.velocity = new Vector2(xMove * speed, rb.velocity.y);
    }

    private void CheckMoveDir()
    {
        //get collider bounds
        var bounds = collider2D.bounds;
        //get bounds center
        var center = bounds.center;
        //get bounds extents
        var extents = bounds.extents;
        //get bounds corners
        var bottomLeft = new Vector2(center.x - extents.x, center.y - extents.y);
        var bottomRight = new Vector2(center.x + extents.x, center.y - extents.y);
        //center left and right
        var centerLeft = new Vector2(center.x - extents.x, center.y);
        var centerRight = new Vector2(center.x + extents.x, center.y);
        //check walls 
       var leftHit = Physics2D.Raycast(centerLeft, Vector2.left, 0.1f, groundLayer);
       var rightHit = Physics2D.Raycast(centerRight, Vector2.right, 0.1f, groundLayer);
       var leftBottomHit = Physics2D.Raycast(bottomLeft, Vector2.down, 0.1f, groundLayer);  
       var rightBottomHit = Physics2D.Raycast(bottomRight, Vector2.down, 0.1f, groundLayer);
       //draw debug rays
         Debug.DrawRay(centerLeft, Vector2.left * 0.1f, Color.red);
         Debug.DrawRay(centerRight, Vector2.right * 0.1f, Color.red);
         Debug.DrawRay(bottomLeft, Vector2.down * 0.1f, Color.red);
         Debug.DrawRay(bottomRight, Vector2.down * 0.1f, Color.red);
        if(leftHit.collider != null || leftBottomHit.collider == null)
            moveDirection = 1;
        else if(rightHit.collider != null || rightBottomHit.collider == null)
            moveDirection = -1;

    }

    public override void TakeDamage(int damage)
    {
        
    }
}
