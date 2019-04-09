using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    private const int LayerMin = 7;
    [SerializeField] private bool affectLayerOrder = false;
    [SerializeField] private int fromOrderInLayer;
    [SerializeField] private int toOrderInLayer;
    [SerializeField] private bool affectZ = false;
    [SerializeField] private int fromZ;
    [SerializeField] private int toZ;
    
    private void OnTriggerEnter2D(Collider2D playerCollider)
    {
        var player = playerCollider.gameObject;
        if (NotPlayer(playerCollider, player)) return;

        if (affectLayerOrder && player.layer == fromOrderInLayer + LayerMin)
        {
            player.GetComponent<Renderer>().sortingOrder = toOrderInLayer;
            player.layer = toOrderInLayer + LayerMin;
        }
        
        var playerTransform = player.GetComponentInParent<Rigidbody2D>().transform;
        if (affectZ && playerTransform.position.z.Equals(fromZ))
        {
            playerTransform.Translate(0 , 0, toZ);
        }
    }

    private static bool NotPlayer(Collider2D playerCollider, GameObject player)
    {
        return !player.CompareTag("Player") || !(playerCollider is BoxCollider2D);
    }
}
