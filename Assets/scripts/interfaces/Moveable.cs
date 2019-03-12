using UnityEngine;

public abstract class Moveable : MonoBehaviour
{
    private Rigidbody2D rigidBody;

    protected virtual void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    public virtual void Move(Vector2 velocity, Vector2 linearSteering)
    {
        rigidBody.velocity = velocity * (Time.deltaTime + .5f) * linearSteering * Time.deltaTime;
    }
    
    public virtual void Move(Vector2 velocity)
    {
        rigidBody.velocity = velocity;
    }

    public abstract void FlipSpriteIfNeeded();
}
