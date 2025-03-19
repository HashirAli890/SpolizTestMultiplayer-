using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;

public class RoomDetails : MonoBehaviour
{
    public RoomInfo Info;
    public TMP_Text RoomNameText;
    [SerializeField] Button RoomButton;
    private void Start()
    {
        RoomButton.onClick.AddListener(()=> 
        {
            EventHandler.JoinRoom?.Invoke(Info.Name);
        });
    }
    public void AssignDetails(string roomName,RoomInfo info)
    {
        Info = info;
        RoomNameText.text = roomName;
    }

}
