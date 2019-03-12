using UnityEngine;

public class LayerOrderGate : MonoBehaviour
{
    private const int layerMin = 7;
    [SerializeField] int toOrderInLayer;
    
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player") && collider is BoxCollider2D)
        {        
            collider.gameObject.GetComponent<Renderer>().sortingOrder = toOrderInLayer;
            collider.gameObject.layer = toOrderInLayer + layerMin;
        }
    }
}
