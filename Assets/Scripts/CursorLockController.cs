using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorLockController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame  
    void Update()
    {
        // Toggle state upon hitting cancel
        if (Input.GetButtonDown("Cancel")) {
            // Change to none if locked
            if (Cursor.lockState == CursorLockMode.Locked) {
                Cursor.lockState = CursorLockMode.None;
            }
            // change to locked otherwise
            else {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}
