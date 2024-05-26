using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTag : MonoBehaviour
{
    [SerializeField] private string tagName;

    [SerializeField] private UnityEngine.Events.UnityEvent events;

    [SerializeField] private UnityEngine.Events.UnityEvent exitEvents;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == tagName)
        {
            events.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == tagName)
        {
            exitEvents.Invoke();
        }
    }
}
