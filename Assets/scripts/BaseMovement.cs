using AI;
using UnityEngine;

public abstract class BaseMovement : Moveable
{
    [SerializeField] protected float maxSpeed;
    protected MovementBehaviour movementBehaviour;
    protected Rigidbody2D body;
    protected SpriteRenderer spriteRenderer;
    protected Animator animator;
    protected Vector2 nextMovement;

    protected override void Awake()
    {
        base.Awake();
        movementBehaviour = GetComponent<MovementBehaviour>();
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
    }

    protected virtual void FixedUpdate()
    {
        Move(nextMovement);
       
        UpdateAnimator();
        
        FlipSpriteIfNeeded();
    }

    protected virtual void UpdateAnimator()
    {
        animator.SetFloat("vertical", nextMovement.y);
        animator.SetFloat("horizontal", nextMovement.x);

        if (nextMovement.magnitude != 0f)
        {
            animator.SetFloat("lastVertical", nextMovement.y);
            animator.SetFloat("lastHorizontal", nextMovement.x);    
        }
    }

    public override void FlipSpriteIfNeeded()
    {
        if (! nextMovement.Equals(Vector2.zero)) {
            spriteRenderer.flipX = nextMovement.x < 0;
        }
    }
}