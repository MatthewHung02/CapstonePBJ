using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapToGrid : MonoBehaviour
{
    public float snapDistance = 0.5f;  // Maximum distance to snap
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            SnapPoint snapPoint = contact.otherCollider.GetComponent<SnapPoint>();
            if (snapPoint != null && !snapPoint.isOccupied)
            {
                float distance = Vector3.Distance(contact.point, snapPoint.transform.position);
                if (distance <= snapDistance)
                {
                    SnapObjectToPoint(snapPoint);
                    break;
                }
            }
        }
    }

    void SnapObjectToPoint(SnapPoint snapPoint)
    {
        rb.isKinematic = true;  // Disable physics
        transform.position = snapPoint.transform.position;
        transform.rotation = snapPoint.transform.rotation;
        snapPoint.isOccupied = true;
    }

    void OnCollisionExit(Collision collision)
    {
        SnapPoint snapPoint = collision.gameObject.GetComponent<SnapPoint>();
        if (snapPoint != null && snapPoint.isOccupied)
        {
            snapPoint.isOccupied = false;
            rb.isKinematic = false;  // Re-enable physics
        }
    }
}
