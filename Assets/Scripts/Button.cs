using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour, IToggle
{
    [SerializeField] private bool isToggle;
    [SerializeField] private List<GameObject> _toggles;
    private bool _state;
    public void Toggle(bool isOn)
    {
        _state = isOn;
        foreach (var toggle in _toggles)
        {
            toggle.GetComponent<IToggle>()?.Toggle(isOn);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
       Toggle(!_state);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(!isToggle)
            Toggle(!_state);
    }
}
