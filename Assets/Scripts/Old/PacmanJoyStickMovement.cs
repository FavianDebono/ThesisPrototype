using UnityEngine;

public class PacmanSimpleMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 2f;

    [Header("Screen Bounds")]
    public float minX = -0.45f;
    public float maxX = 0.45f;
    public float minY = -0.45f;
    public float maxY = 0.45f;

    void Update()
    {
        // Read input (WASD, arrow keys, thumbstick)
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        // Move Pac-Man in 2D
        Vector3 movement = new Vector3(x, y, 0f);
        transform.localPosition += movement * speed * Time.deltaTime;

        // Clamp Pac-Man to the screen
        Vector3 pos = transform.localPosition;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        pos.z = 0f;

        transform.localPosition = pos;
    }
}
