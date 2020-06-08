using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationController : MonoBehaviour
{
    /// <summary>
    /// Sensitivity value for movement.
    /// </summary>
    public float sensitivity = 100.0f;
    /// <summary>
    /// Minimum angle of pitch (up and down).
    /// </summary>
    public float minVerticalAngle = -80f;
    /// <summary>
    /// Maximum angle of pitch (up and down).
    /// </summary>
    public float maxVerticalAngle = 80f;

    /// <summary>
    /// Object to change pitch (up and down) when moving mouse.
    /// </summary>
    public Transform pitchTarget;
    /// <summary>
    /// object ot change yaw (left and right) when moving mouse.
    /// </summary>
    public Transform yawTarget;

    /// <summary>
    /// Current pitch attitude
    /// </summary>
    private float pitch;
    /// <summary>
    /// Current yaw attitude
    /// </summary>
    private float yaw;

    // Update is called once per frame
    void Update()
    {
        // Get the player input change
        float pitchRotation = Input.GetAxis("Mouse Y");
        float yawRotation = Input.GetAxis("Mouse X");

        // Change player input into pitch and yaw units
        pitch +=  pitchRotation * -1 * Time.deltaTime * this.sensitivity;
        yaw +=  yawRotation * Time.deltaTime * this.sensitivity;

        // Bound pitch values
        pitch = Mathf.Clamp(pitch, this.minVerticalAngle, this.maxVerticalAngle);

        // Rotate target object by modified vector
        this.pitchTarget.localRotation = Quaternion.AngleAxis(pitch, Vector3.right);
        this.yawTarget.localRotation = Quaternion.AngleAxis(yaw, Vector3.up);
    }
}
