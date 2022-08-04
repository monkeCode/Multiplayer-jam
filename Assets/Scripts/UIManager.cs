using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
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
        _diePanel.SetActive(false);
    }
}
