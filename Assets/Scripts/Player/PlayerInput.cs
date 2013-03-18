using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour
{
    public float mouseSensitivity = 300;
    public bool showCursor = true;

    void Start()
    {
        InputHandler.mouseSensitivity = mouseSensitivity;
//        Screen.lockCursor = true;
        Screen.showCursor = showCursor;
        StartCoroutine(InputHandler.LockCursor());
        StartCoroutine(InputHandler.UpdateInput());
    }
}