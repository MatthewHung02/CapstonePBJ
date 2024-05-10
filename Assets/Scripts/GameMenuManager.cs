using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameMenuManager : MonoBehaviour
{
    public Transform head;
    public float spawnDistance = 2;
    public GameObject menu;
    public InputActionProperty showButton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (showButton.action.WasPressedThisFrame())
        {
            menu.SetActive(!menu.activeSelf);

            menu.transform.position = head.position + new Vector3(head.forward.x, 0, head.forward.z).normalized * spawnDistance;
        }
        //Makes menu look at viewer's head
        menu.transform.LookAt(new Vector3 (head.position.x, menu.transform.position.y, head.position.z)); //This doesn't work, but I'm keeping it here for reference
        menu.transform.forward *= -1;   //The menu will show up inversed without this
    }
}
