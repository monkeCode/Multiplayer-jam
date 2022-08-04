using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class LvlManager : MonoBehaviourPun
{
    [SerializeField] private Transform _player1Pos;
    [SerializeField] private Transform _player2Pos;
    void Start()
    {
        photonView.RPC(nameof(ResetPlayers), RpcTarget.All);
    }
    [PunRPC]
    public void ResetPlayers()
    {
        if(Server.Instance.Player1!= null)
            Server.Instance.Player1.transform.position = _player1Pos.position;
        if(Server.Instance.Player2 != null)
            Server.Instance.Player2.transform.position = _player2Pos.position;
    }
}
