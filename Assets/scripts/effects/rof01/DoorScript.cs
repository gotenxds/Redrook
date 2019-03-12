using UnityEngine;

namespace effects.rof01
{
    public class DoorScript : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;
        private new BoxCollider2D collider;
        private float transparentValue = 15;
        private bool check = true;
        private void Awake()
        {
            collider = GetComponent<BoxCollider2D>();
            spriteRenderer = GameObject.Find("Player").GetComponentInChildren<SpriteRenderer>();
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!check) return;
        
            var player = other.gameObject;

            if (player.CompareTag("Player"))
            {
                var distanceFromCenter = Vector2.Distance(transform.TransformPoint(collider.offset), other.transform.TransformPoint(other.offset));
                var maxDistance = collider.size.y / 2;
                var alpha = distanceFromCenter / maxDistance;
                var color = spriteRenderer.color;

                color = new Color(color.g, color.g, color.b, alpha);
                spriteRenderer.color = color;

                if (alpha * 255 <= transparentValue)
                {
                    player.GetComponentInParent<CharacterMovement>().IsFrozen = true;
                    FadeManager.FadeIn();
                    check = false;
                }
            }
        }
    }
}
