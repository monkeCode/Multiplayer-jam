using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class LobbyLoader : MonoBehaviourPun,IToggle
{
  private  int _count;
    public void Toggle(bool isOn)
    {
        if (photonView.IsMine)
        {
            _count += isOn ? 1 : -1;
            if (_count == 2)
            {
                Server.Instance.LoadFirstLvl();
            }
        }
    }
}
