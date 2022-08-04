using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public void RestartGame()
    {
        Server.Instance.RestartLvl();
    }

    public void Leave()
    {
        Server.Instance.ToMainMenu();
    }
}
