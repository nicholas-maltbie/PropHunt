using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationController : MonoBehaviour
{
    /// <summary>
    /// Sensitivity value for movement.
    /// </summary>
    public float sensitivity = 10.0f;
    public float minVerticalAngle = -80f;
    public float maxVerticalAngle = 90f;

    /// <summary>
    /// Object to change pitch (up and down) when moving mouse
    /// </summary>
    public Transform pitchTarget;
    /// <summary>
    /// object ot change yaw (left and right) when moving mouse
    /// </summary>
    public Transform yawTarget;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Get the player input change
        float pitchRotation = Input.GetAxis("Mouse Y");
        float yawRotation = Input.GetAxis("Mouse X");

        // Change player input into pitch and yaw units
        Vector3 pitchChange =  new Vector3(pitchRotation * -1, 0, 0) * Time.deltaTime * this.sensitivity;
        Vector3 yawChange =  new Vector3(0, yawRotation, 0) * Time.deltaTime * this.sensitivity;

        // Rotate target object by modified vector
        this.pitchTarget.eulerAngles = this.pitchTarget.eulerAngles + pitchChange;
        this.yawTarget.eulerAngles = this.yawTarget.eulerAngles + yawChange;
    }
}
