using UnityEngine;

public class DisappearAfterFalling : MonoBehaviour
{
    public float fallTimeThreshold = 7.0f; // Time in seconds before the object disappears
    private float fallTime = 0.0f;
    private bool isFalling = false;

    void Update()
    {
        // Check if the object is falling (i.e., its Rigidbody is moving downwards)
        if (GetComponent<Rigidbody>().velocity.y < 0)
        {
            if (!isFalling)
            {
                isFalling = true; // The object has started falling
                fallTime = 0.0f; // Reset fall time
            }

            // Increment the fall time
            fallTime += Time.deltaTime;

            // Check if the fall time exceeds the threshold
            if (fallTime >= fallTimeThreshold)
            {
                // Make the object disappear
                gameObject.SetActive(false);
            }
        }
        else
        {
            // If the object is not falling, reset the fall time and status
            isFalling = false;
            fallTime = 0.0f;
        }
    }
}
