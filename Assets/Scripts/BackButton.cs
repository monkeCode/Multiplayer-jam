using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BackButton : MonoBehaviour, IToggle
{
    [SerializeField] private LvlChanger changer;
    public void Toggle(bool isOn)
    {
        changer.Back();
    }
}
