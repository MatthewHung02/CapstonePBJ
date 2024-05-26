using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreadSpawner : MonoBehaviour
{
    public GameObject breadSlicePrefab;
    public Transform spawnPoint;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == transform)
                {
                    SpawnBreadSlice();
                }
            }
        }
    }

    void SpawnBreadSlice()
    {
        Instantiate(breadSlicePrefab, spawnPoint.position, spawnPoint.rotation);
    }
}

