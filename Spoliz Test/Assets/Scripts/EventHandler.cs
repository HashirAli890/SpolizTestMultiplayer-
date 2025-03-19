
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EventHandler : MonoBehaviour
{
    public static Action<Scenes> LoadScene;
    public static Action<string> LoadLoadingScreen;
    public static Action UnloadLoadingScreen;
    public static Action<List<RoomInfo>> RoomListUpdated;
    public static Action<string> CreateRoom;
    public static Action<string> JoinRoom;
    public static Action LeaveRoom;
    public static Action<bool> UpdateGameState;

    public static string PlayerNamePref = "PlayerName";


   
    private void Awake()
    {
       
        DontDestroyOnLoad(this.gameObject);
    }
   
}
