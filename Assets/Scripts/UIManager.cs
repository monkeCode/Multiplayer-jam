using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class UIManager : MonoBehaviourPun
{
    public static UIManager Instance { get; private set; }
    [SerializeField] private GameObject _diePanel;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
        }
        
        DontDestroyOnLoad(gameObject);
    }

    public void RestartGame()
    {
        Server.Instance.RestartLvl();
    }

    public void Leave()
    {
        Server.Instance.ToMainMenu();
    }

    public void ShowDiePanel()
    {
        _diePanel.SetActive(true);
    }
    public void CloseDiePanel()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(CloseRpc), RpcTarget.All);
        }
    }

    [PunRPC]
    private void CloseRpc()
    {
        _diePanel.SetActive(false);
    }
}
