using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRaycaster : MonoBehaviour
{
    public float maxHitDistance = Mathf.Infinity;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, maxHitDistance))
        {
            print("Found an object");
            GameObject hitObject = hit.transform.gameObject;
            Prop prop = hitObject.GetComponent<Prop>();
            if (prop != null)
            {
                print("I'm a prop");
            }
            else
            {
                print("I'm not a prop");
            }
        }
    }
}
