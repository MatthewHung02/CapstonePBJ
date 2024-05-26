using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnerKnife : MonoBehaviour
{
    public float disappearTime = 2f; // Time before the knife disappears
    public float respawnTime = 5f; // Time before the knife respawns
    public float moveSpeed = 2f; // Speed of the knife movement
    public float moveRange = 5f; // Range of the knife movement
    public float interactionRange = 2f; // Range within which the player can interact with the knife
    public KeyCode interactionKey = KeyCode.E; // Key to interact with the knife

    private Vector3 initialPosition; // Store the initial position of the knife
    private bool isDisappeared = false; // Flag to check if the knife has disappeared
    private bool isMovingRight = true; // Flag to determine the direction of movement

    void Start()
    {
        initialPosition = transform.position; // Store the initial position
        Invoke("Disappear", disappearTime); // Call the Disappear method after the disappearTime
    }

    void Update()
    {
        HandleMovement();
        HandlePlayerInteraction();
    }

    void HandleMovement()
    {
        // Move the knife back and forth along the x-axis
        if (isMovingRight)
        {
            transform.position += Vector3.right * moveSpeed * Time.deltaTime;
            if (transform.position.x >= initialPosition.x + moveRange)
                isMovingRight = false;
        }
        else
        {
            transform.position += Vector3.left * moveSpeed * Time.deltaTime;
            if (transform.position.x <= initialPosition.x - moveRange)
                isMovingRight = true;
        }
    }

    void HandlePlayerInteraction()
    {
        // Check if the player is within interaction range
        GameObject player = GameObject.FindWithTag("Player"); // Assuming the player has the tag "Player"
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToPlayer <= interactionRange && !isDisappeared)
            {
                if (Input.GetKeyDown(interactionKey))
                {
                    // Player picks up the knife
                    Debug.Log("Knife picked up by player!");
                    // Add logic for picking up the knife, e.g., adding it to inventory
                    Disappear(); // Make the knife disappear after being picked up
                }
            }
        }
    }

    void Disappear()
    {
        gameObject.SetActive(false); // Disable the knife
        isDisappeared = true; // Set the flag to true
        Invoke("Respawn", respawnTime); // Call the Respawn method after the respawnTime
    }

    void Respawn()
    {
        gameObject.SetActive(true); // Enable the knife
        transform.position = initialPosition; // Reset the position
        isDisappeared = false; // Reset the flag
        Invoke("Disappear", disappearTime); // Call the Disappear method again after the disappearTime
    }
}