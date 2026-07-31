using UnityEngine;


public class JoystickStartGame : MonoBehaviour
{
    public XRJoystickDrive joystick;
    public GameManager gameManager;
    public Renderer joystickRenderer;
    public Material idleMat;
    public Material activeMat;


    [Range(0f, 1f)]
    public float startThreshold = 0.7f;

    bool started = false;

    public void ResetJoystick()
    {
        started = false;
        joystickRenderer.material = idleMat;
    }


    void Update()
    {
        if (started)
            return;


        // Pull joystick DOWN (negative Y)
        if (joystick.value.y < -startThreshold)
        {
            started = true;
            joystickRenderer.material = activeMat;
            gameManager.StartGame();
            

        }
    }
}
