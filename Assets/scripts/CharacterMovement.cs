using UnityEngine;

public class CharacterMovement : BaseMovement
{
    public bool IsFrozen { get; set; }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("movementCollider"))
        {
            Physics2D.IgnoreCollision(collision.collider, GetComponentInChildren<CapsuleCollider2D>());
        }
    }

    protected override void FixedUpdate()
    {
        if (IsFrozen)
        {
            body.velocity = Vector2.zero;

            return;
        }
        
        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");

        nextMovement = new Vector2(horizontal, vertical) * maxSpeed;
        
        base.FixedUpdate();
    }
}