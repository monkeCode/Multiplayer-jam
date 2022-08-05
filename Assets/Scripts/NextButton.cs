using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class NextButton : MonoBehaviour, IToggle
{
    [SerializeField] private LvlChanger lvlChanger;
    public void Toggle(bool isOn)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            lvlChanger.Forward();
        }
    }
}
