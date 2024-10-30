using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyCamera : MonoBehaviour
{
    public float movementSpeed = 10.0f;     // Movement speed
    public float lookSpeed = 2.0f;          // Speed for mouse look sensitivity
    public float shiftMultiplier = 2.5f;    // Multiplier when holding Shift for faster movement
    private float yaw = 0.0f;
    private float pitch = 0.0f;

    void Start()
    {
        // Lock the cursor to the game window and make it invisible
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Check for Shift key to increase speed
        float currentSpeed = movementSpeed * (Input.GetKey(KeyCode.LeftShift) ? shiftMultiplier : 1.0f);

        // Move camera based on WASD input
        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        transform.Translate(direction * currentSpeed * Time.deltaTime);

        // Rotate camera based on mouse movement
        yaw += lookSpeed * Input.GetAxis("Mouse X");
        pitch -= lookSpeed * Input.GetAxis("Mouse Y");

        // Clamp the pitch to avoid flipping the camera
        pitch = Mathf.Clamp(pitch, -90f, 90f);

        // Apply rotation to the camera
        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);

        // Toggle cursor lock with the Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = !Cursor.visible;
        }
    }
}
