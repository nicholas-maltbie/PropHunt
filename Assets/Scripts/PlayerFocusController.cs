using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class PlayerFocusController : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}

[System.Serializable]
public class GainFocus : UnityEvent<PlayerRaycaster, Prop>
{

}

[System.Serializable]
public class LoseFocus : UnityEvent<PlayerRaycaster, Prop>
{

}
