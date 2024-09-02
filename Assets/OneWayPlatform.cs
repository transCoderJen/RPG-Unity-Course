using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    private PlatformEffector2D effector;
    public float waitTime = 0.5f;
    private float timer;

    void Start()
    {
        effector = GetComponent<PlatformEffector2D>();
    }

    void Update()
    {
        // If the player is pressing the down arrow and jump at the same time, allow them to drop through the platform
        if (Input.GetKey(KeyCode.S) && Input.GetKeyDown(KeyCode.Space))
        {
            effector.rotationalOffset = 180f;
            timer = waitTime; // Start the timer
        }

        // Timer countdown to reset the platform so the player can stand on it again
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                effector.rotationalOffset = 0f;
            }
        }
    }
}
