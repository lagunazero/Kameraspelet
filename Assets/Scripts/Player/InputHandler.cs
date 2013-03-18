using UnityEngine;
using System.Collections;

public static class InputHandler
{
    public static float mouseSensitivity;

    private static Vector3 deltaMousePosition = Vector3.zero;
    public static Vector3 DeltaMousePosition { get { return deltaMousePosition; } }
    private static Vector3 mousePosition = new Vector3(Screen.width / 2, Screen.height / 2, 0);
    public static Vector3 MousePosition { get { return mousePosition; } }

    private static Vector3 deltaActualMousePosition = Vector3.zero;
    public static Vector3 DeltaActualMousePosition { get { return deltaActualMousePosition; } }

    private static float mouseDrag = 1;

    public static float mouseLockFrequency = 0.5f;

    private static Vector3 lastMousePosition = new Vector3(Screen.width / 2, Screen.height / 2, 0);

    private static bool lockMouse;
    public static bool analogue = true;
	
	public static float stickSensitivity = 10;
	
    private static WaitForFixedUpdate wait = new WaitForFixedUpdate();

    public static void LockMouse(bool value)
    {
        lockMouse = value;
        lastMousePosition = mousePosition;
        deltaMousePosition = Vector3.zero;
    }

    /*	void Start ()
        {
            Screen.showCursor = false;
            Screen.lockCursor = true;
            //mousePosition = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);
            mousePosition = Input.mousePosition;
            deltaMousePosition = Vector3.zero;
        }*/

    // Update is called once per frame
    public static IEnumerator UpdateInput()
    {
        while (true)
        {
			Vector2 stickVector = new Vector2(Input.GetAxis("LookStickX"),Input.GetAxis("LookStickY"));
			stickVector = stickVector.normalized * Mathf.Pow(stickVector.magnitude,2);
            deltaActualMousePosition.x = Input.GetAxis("Mouse X") * mouseSensitivity * Time.fixedDeltaTime +stickVector.x * stickSensitivity;
            deltaActualMousePosition.y = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.fixedDeltaTime +stickVector.y * stickSensitivity;

            if (!lockMouse)
            {
                mousePosition.x = Mathf.Clamp(mousePosition.x + deltaActualMousePosition.x, 0, Screen.width);
                mousePosition.y = Mathf.Clamp(mousePosition.y + deltaActualMousePosition.y, 0, Screen.height);

                deltaMousePosition = mousePosition - lastMousePosition;
                lastMousePosition = mousePosition;
            }
            yield return wait;
        }
    }

    public static IEnumerator LockCursor()
    {
        while (true)
        {
            //This is done since the mouse lock apparently breaks every so often,
            //but refreshing it resets the mouse position to the centre of the screen.
//            Screen.lockCursor = true;
            yield return new WaitForSeconds(mouseLockFrequency);
        }
    }

    public static void ApplyMouseDrag(float drag)
    {
        mouseDrag += drag;
    }

    /// <summary>Sets the mouse position at the given position.
    /// This also sets the delta position to 0.</summary>
    public static void SetMousePosition(Vector3 position)
    {
        lastMousePosition = mousePosition = position;
    }

    public static void MoveMousePosition(Vector3 movement)
    {
        mousePosition += movement;
    }
}