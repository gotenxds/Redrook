using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class script : MonoBehaviour
{
    Vector3 x;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        x = collision.transform.position;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other is BoxCollider2D)
        {
            Debug.Log(x - other.transform.position);
        }
    }
}
