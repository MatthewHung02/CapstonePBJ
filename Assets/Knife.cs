using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Knife : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        // Example condition: knife hits a target
        if (collision.gameObject.CompareTag("Target"))
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // Example condition: knife falls off the screen
        if (transform.position.y < -10)
        {
            Destroy(gameObject);
        }
    }
}