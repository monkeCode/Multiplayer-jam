using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LvlManager : MonoBehaviour
{
    [SerializeField] private Transform _player1Pos;
    [SerializeField] private Transform _player2Pos;
    void Start()
    {
        ResetPlayers();
    }

    public void ResetPlayers()
    {
        if(Server.Instance.Player1!= null)
            Server.Instance.Player1.transform.position = _player1Pos.position;
        if(Server.Instance.Player2 != null)
            Server.Instance.Player2.transform.position = _player2Pos.position;
    }
}
