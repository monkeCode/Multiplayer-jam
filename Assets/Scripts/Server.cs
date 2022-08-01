using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Server : MonoBehaviourPunCallbacks
{
    public static Server Instance { get; private set; }
    private bool _sceneLoaded;
    [SerializeField] private List<InventoryItem> _items;
    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster() was called by PUN.");
        PhotonNetwork.JoinOrCreateRoom("Test", new RoomOptions {MaxPlayers = 2}, TypedLobby.Default);
    }
    public override void OnCreatedRoom()
    {
        Debug.Log("Created room: " + PhotonNetwork.CurrentRoom.Name);
        base.OnCreatedRoom();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room: " + PhotonNetwork.CurrentRoom.Name);
        base.OnJoinedRoom();
        var player =  PhotonNetwork.Instantiate("Player", new Vector3(1, 1, 0), Quaternion.identity);
        Camera.main.GetComponent<CinemachineVirtualCamera>().Follow = player.transform;
    }
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

    void Start()
    {
        Debug.Log(" started");
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.IsMessageQueueRunning = false;
        PhotonNetwork.ConnectUsingSettings();
    }
    

    public void CreateRoom(string roomName)
    {
        PhotonNetwork.CreateRoom(roomName, new RoomOptions{ MaxPlayers = 2}, TypedLobby.Default);
    }
    
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    private void LoadLvl(string name)
    {
        _sceneLoaded = false;
        PhotonNetwork.LoadLevel(name);
        _sceneLoaded = true;
    }
    
    public IEnumerator MoveToGameScene(string nameScene)
    {
        PhotonNetwork.IsMessageQueueRunning = false;
        LoadLvl(nameScene);
        while(_sceneLoaded == false)
        {
            yield return null;
        }
        PhotonNetwork.IsMessageQueueRunning = true;
    }

    public InventoryItem GetItem(int itemHash)
    {
        return _items.Find(x => x.GetHashCode() == itemHash);
    }
}
