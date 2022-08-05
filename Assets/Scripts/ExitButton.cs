using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitButton : MonoBehaviour, IToggle
{
   
    public void Toggle(bool isOn)
    {
        Server.Instance.ToMainMenu();
    }
}
