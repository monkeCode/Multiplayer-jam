using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Server : MonoBehaviourPunCallbacks
{
    public static Server Instance { get; private set; }
    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster() was called by PUN.");
        PhotonNetwork.JoinOrCreateRoom("Test", new RoomOptions {MaxPlayers = 2}, TypedLobby.Default);
    }
    public override void OnCreatedRoom()
    {
        Debug.Log("Created room: " + PhotonNetwork.CurrentRoom.Name);
        base.OnCreatedRoom();
        SceneManager.LoadScene("SampleScene");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room: " + PhotonNetwork.CurrentRoom.Name);
        base.OnJoinedRoom(); 
        //PhotonNetwork.Instantiate("Player", new Vector3(1, 1, 0), Quaternion.identity);
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

    public void ChangeScene(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }
    
}
