using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A Pusher will push all rigidbodies that the character controller touches
/// Code snippet based on solution from titegtnodI - Aug 19, 2012
/// http://answers.unity.com/answers/304497/view.html
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class Pusher : MonoBehaviour
{
    /// <summary>
    /// Power of the push
    /// </summary>
    public float pushPower = 2.0f;
    /// <summary>
    /// Weight (for fore calculation) of character
    /// </summary>
    public float weight = 6.0f;
    /// <summary>
    /// Force of gravity when pushing objects
    /// </summary>
    public float gravity = 20.0f;

    /// <summary>
    /// OnControllerColliderHit is called when the controller hits a
    /// collider while performing a Move.
    /// </summary>
    /// <param name="hit">The ControllerColliderHit data associated with this collision.</param>
    public void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        Vector3 force;

        // no rigidbody
        if (body == null || body.isKinematic) {
            return;
        }

        // We use gravity and weight to push things down, we use
        // our velocity and push power to push things other directions
        if (hit.moveDirection.y < -0.3f) {
            force = new Vector3 (0, -0.5f, 0) * this.gravity * this.weight;
        }
        else {
            force = hit.controller.velocity * this.pushPower;
        }

        // Apply the push
        body.AddForceAtPosition(force, hit.point);
    }
}
