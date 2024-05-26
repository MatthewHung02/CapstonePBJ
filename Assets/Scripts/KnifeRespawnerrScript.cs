using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class KnifeRespawner : MonoBehaviour
{
    public GameObject knifePrefab;  // Reference to the knife prefab
    public Transform spawnPoint;    // Reference to the spawn point

    private GameObject currentKnife;

    void Start()
    {
        RespawnKnife();
    }

    void Update()
    {
        if (currentKnife == null)
        {
            RespawnKnife();
        }
    }

    void RespawnKnife()
    {
        currentKnife = Instantiate(knifePrefab, spawnPoint.position, spawnPoint.rotation);
    }
}