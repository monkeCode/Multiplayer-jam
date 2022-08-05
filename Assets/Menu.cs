using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;

    public void Connect()
    {
        Server.Instance.SetRoomName(_text.text);
        Server.Instance.JoinOrCreateRoom();
    }
    public void Exit()
    {
        Application.Quit();
    }
}
