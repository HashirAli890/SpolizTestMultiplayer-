using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonConnection : MonoBehaviourPunCallbacks,ILobbyCallbacks,IInRoomCallbacks
{
    public static PhotonConnection Instance;
    string _roomName;
    [SerializeField] float ReconnectingWait;

     bool isReconnecting = false;
    [SerializeField] float reconnectAttemptDelay = 2f;
    [SerializeField] int maxReconnectAttempts = 5;
    private int currentReconnectAttempts = 0;
    bool gameOver;



    void Awake()
    {
        if (PhotonConnection.Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);

        }
        DontDestroyOnLoad(this.gameObject);
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        EventHandler.CreateRoom += CreateRoomInLobby;
        EventHandler.JoinRoom += JoinRoom;
        EventHandler.LeaveRoom += LeaveRoom;
        EventHandler.UpdateGameState += UpdateGameState;
        PhotonNetwork.ConnectUsingSettings();   
    }
    void OnDisable() 
    {
        EventHandler.CreateRoom -= CreateRoomInLobby;
        EventHandler.JoinRoom -= JoinRoom;
        EventHandler.LeaveRoom -= LeaveRoom;
        EventHandler.UpdateGameState -= UpdateGameState;
    }
    private void JoinRoom(string Name)
    {
        PhotonNetwork.JoinRoom(Name);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected To Server");
        if (isReconnecting && PhotonNetwork.InRoom)
        {
            PhotonNetwork.RejoinRoom(PhotonNetwork.CurrentRoom.Name);
        }
        else if (!PhotonNetwork.InRoom)
        {
            PhotonNetwork.JoinLobby(); 
        }
    }
    public override void OnLeftLobby()
    {
        Debug.Log("Lobby Left");
    }
    public override void OnJoinedLobby()
    {
        Debug.Log("Lobby Joined");
        PhotonNetwork.NickName = PlayerPrefs.GetString(EventHandler.PlayerNamePref);
        gameOver = false;
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("Room list updated Callback");
        EventHandler.RoomListUpdated?.Invoke(roomList);
    }
    void CreateRoomInLobby(string RoomName)
    {
        Debug.Log($"Room Name {RoomName} ");
        if (PhotonNetwork.IsConnectedAndReady)
        {
            _roomName = RoomName;
            RoomOptions RoomOpt = new RoomOptions()
            {
                CleanupCacheOnLeave = false,
                PlayerTtl = -1,
                IsVisible = true,
                IsOpen = true,
                MaxPlayers = 2,
                PublishUserId = true,
                EmptyRoomTtl = 300

            };
            PhotonNetwork.KeepAliveInBackground = 600f;
            PhotonNetwork.JoinOrCreateRoom(RoomName, RoomOpt, null);
        }
        else
        {
            Debug.Log($"Reconecting");
            StartCoroutine(WaitToReconnect());
        }

    }
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room");
        if (SceneManager.GetActiveScene().name != Scenes.GamePlay.ToString())
            EventHandler.LoadLoadingScreen?.Invoke("Wait For Other Players To Join");
        else
            EventHandler.UnloadLoadingScreen?.Invoke();




    }
    IEnumerator WaitToReconnect()
    {
        yield return new WaitForSeconds(ReconnectingWait);
        CreateRoomInLobby(_roomName);
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Player NickName {newPlayer.NickName}");
        if (SceneManager.GetActiveScene().name != Scenes.GamePlay.ToString())
        {
            EventHandler.LoadScene(Scenes.GamePlay);
        }
        Debug.Log("Unloading Screen");
            EventHandler.UnloadLoadingScreen?.Invoke();

    }
    public override void OnDisconnected(DisconnectCause cause)
    {
       
        if (cause != DisconnectCause.DisconnectByClientLogic) 
        {
            StartCoroutine(AttemptReconnect());
        }
    }

    private IEnumerator AttemptReconnect()
    {
        isReconnecting = true;
     
        EventHandler.LoadLoadingScreen?.Invoke("Connection Failed. Trying To Reconnect");
        while (isReconnecting && currentReconnectAttempts < maxReconnectAttempts)
        {
            if (PhotonNetwork.ReconnectAndRejoin())
            {
                yield break; 
            }

            currentReconnectAttempts++;
            yield return new WaitForSeconds(reconnectAttemptDelay);
        }

        isReconnecting = false;
        ReturnToLobby();
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        if(!isReconnecting)
            ReturnToLobby();

    }
    private void ReturnToLobby()
    {

        EventHandler.LoadLoadingScreen?.Invoke("Reconnection failed. Returning to lobby...");
        PhotonNetwork.JoinLobby();
        EventHandler.LoadScene(Scenes.MainMenu);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log(gameOver);
        if (gameOver) 
        {
            Invoke(nameof(WaitForSwitchRoom), 2f);
          
        }
        else
        {
            EventHandler.LoadLoadingScreen?.Invoke("Please Wait Player Get Disconnected");
        }
    }
    void WaitForSwitchRoom()
    {
        LeaveRoom();
    }
    public void LeaveRoom() 
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
    }
    public override void OnLeftRoom()
    {
        EventHandler.LoadScene(Scenes.MainMenu);
        PhotonNetwork.JoinLobby();
    }
    public void UpdateGameState(bool Game)
    {
        gameOver = Game;
    }
  

}
