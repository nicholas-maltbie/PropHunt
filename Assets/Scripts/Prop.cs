using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : MonoBehaviour
{
    private bool isHighlighted = false;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ToggleFocus()
    {
        SetFocus(!isHighlighted);
    }

    void SetFocus(bool focus)
    {
        if (focus)
        {
            print("Defocusing");
        }
        else
        {
            print("Focusing");
        }

        isHighlighted = !isHighlighted;
    }

}
