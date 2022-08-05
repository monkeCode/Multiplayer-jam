using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class LvlChanger : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private TextMeshProUGUI _text;

    private void Start()
    {
        UpdateText();
    }

    public void Back()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Server.Instance.CurrentLvl--;
            if (Server.Instance.CurrentLvl < 0)
                Server.Instance.CurrentLvl = Server.Instance.CountLvl - 2;
            UpdateText();
        }
    }
    public void Forward()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Server.Instance.CurrentLvl++;
            if (Server.Instance.CurrentLvl >= Server.Instance.CountLvl-1)
                Server.Instance.CurrentLvl = -1;
            UpdateText();
        }
    }

    private void UpdateText()
    {
        if (Server.Instance.CurrentLvl + 1 == 0)
            _text.text = "Current lvl: Tutorial";
        else
        {
            _text.text = "Current lvl: " + (Server.Instance.CurrentLvl + 1);
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_text.text);
        }
        else
        {
            _text.text = (string) stream.ReceiveNext();
        }
    }
}
