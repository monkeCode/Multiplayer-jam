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
    [SerializeField] private string[] _scenes;
    [SerializeField] private string _roomName;
    public string RoomName => _roomName;
    private int _currentScene = 0;
    public Player Player1;
    public Player Player2;
    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster() was called by PUN.");
        PhotonNetwork.JoinOrCreateRoom(_roomName, new RoomOptions {MaxPlayers = 2}, TypedLobby.Default);
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
        
        if (PhotonNetwork.IsMasterClient)
        {
            var player =  PhotonNetwork.Instantiate("Player1" , new Vector3(1, 1, 0), Quaternion.identity);
            Camera.main.GetComponent<CinemachineVirtualCamera>().Follow = player.transform;
            Player1 = player.GetComponent<Player>();
        }
        else
        {
            var player =  PhotonNetwork.Instantiate("Player2" , new Vector3(1, 1, 0), Quaternion.identity);
            Camera.main.GetComponent<CinemachineVirtualCamera>().Follow = player.transform;
            Player2 = player.GetComponent<Player>();
            photonView.RPC(nameof(AddSecondPlayer), RpcTarget.MasterClient);
        }
    }

    [PunRPC]
    private void AddSecondPlayer()
    {
        if (photonView.IsMine)
        {
            Player2 = GameObject.Find("Player2(Clone)").GetComponent<Player>();
        }
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
        while(!_sceneLoaded)
        {
            yield return null;
        }
        PhotonNetwork.IsMessageQueueRunning = true;
        
    }
    public Transform GetLookingObject( )
    {
        try
        {
            return PhotonNetwork.IsMasterClient ? Player1?.transform : Player2?.transform;
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return null;
        }
    }
    public InventoryItem GetItem(string itemName)
    {
        return Instantiate(_items.Find(x => x.ItemName == itemName));
    }
    
    
    public void RestartLvl()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            UpdatePlayers();
            StartCoroutine(MoveToGameScene(SceneManager.GetActiveScene().name));
        }
    }

    public void LoadFirstLvl()
    {
        _currentScene = -1;
        LoadNextLvl();
    }
    public void LoadNextLvl()
    {
        UpdatePlayers();
        StartCoroutine(MoveToGameScene(_scenes[++_currentScene]));
    }
    public void ToMainMenu()
    {
        
    }
    
    private void UpdatePlayers()
    {
        if (Player1 != null)
        {
            Player1.transform.parent = null;
            DontDestroyOnLoad(Player1.gameObject);
            //Player1.GetComponent<PhotonView>().RPC(nameof(Player1.SetNonDestroyable), RpcTarget.All);
            Player1?.Heal(Player1.MaxHp);
            Player1?.GetComponent<PhotonView>()?.RPC(nameof(Player1.DeleteActiveItem), RpcTarget.All);
        }
        if (Player2 != null)
        {
            Player2.transform.parent = null;
            DontDestroyOnLoad(Player2.gameObject);
            //Player2.GetComponent<PhotonView>().RPC(nameof(Player2.SetNonDestroyable), RpcTarget.All);
            Player2?.Heal(Player2.MaxHp);
            Player2?.GetComponent<PhotonView>()?.RPC(nameof(Player2.DeleteActiveItem), RpcTarget.All);
        }
        
    }
}
