using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;


public class PhotonRoomHandler : MonoBehaviour
{
    private Dictionary<string, GameObject> roomListItems = new Dictionary<string, GameObject>();
    [SerializeField] GameObject roomListItemPrefab;
    [SerializeField] Transform roomListContent;
    [SerializeField] float ReconnectingWait;
    string _roomName;
    [SerializeField] Button CreateRoomPanelButton;
    [SerializeField] Button BackFromCreateRoom;
    [SerializeField] Button CreateRoom;
    [SerializeField] GameObject CreateRoomPanel;
    [SerializeField] TMP_InputField CreateRoomInputField;



    void OnEnable() 
    {
        EventHandler.RoomListUpdated += OnRoomListUpdate;
        CreateRoomPanelButton.onClick.AddListener(() => 
        {
            CreateRoomPanel.SetActive(true);
            CreateRoom.gameObject.SetActive(false);
            CreateRoomInputField.text = null;
          
        });
        CreateRoom.onClick.AddListener(() =>
        {
            if (!string.IsNullOrEmpty(CreateRoomInputField.text))
            {
                EventHandler.CreateRoom?.Invoke(CreateRoomInputField.text);
                CreateRoomPanel.SetActive(false);
               // CreateRoomInLobby(CreateRoomInputField.text);
            }
        });
        BackFromCreateRoom.onClick.AddListener(()=> 
        {
            CreateRoomPanel.SetActive(false);
        });
    }
    public void OnEnteredName() 
    {
        if (!string.IsNullOrEmpty(CreateRoomInputField.text))
        {
            CreateRoom.gameObject.SetActive(true);
        }
        else 
        {
            CreateRoom.gameObject.SetActive(false);
        }
    }
    void OnDisable() 
    {
        EventHandler.RoomListUpdated -= OnRoomListUpdate;
    }
    public void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ClearRoomListUI();
        Debug.Log("Room list updated");

        Debug.Log($"Room Count {roomList.Count}");
        foreach (RoomInfo room in roomList)
        {
            if (roomListItems.ContainsKey(room.Name))
            {
                if (!room.IsOpen || !room.IsVisible)
                {
                    Destroy(roomListItems[room.Name]);
                    roomListItems.Remove(room.Name);
                }
                else
                {

                    UpdateRoomListItem(room);
                }
            }
        }

        // Add new room entries
        foreach (RoomInfo room in roomList)
        {
            if (room.IsOpen && room.IsVisible && !roomListItems.ContainsKey(room.Name))
            {
                GameObject newRoomItem = Instantiate(roomListItemPrefab, roomListContent);
                RoomDetails roomListItem = newRoomItem.GetComponent<RoomDetails>();
                if (roomListItem != null)
                {
                    roomListItem.AssignDetails(room.Name, room);
                }
                roomListItems.Add(room.Name, newRoomItem);
            }
        }
    }

    void UpdateRoomListItem(RoomInfo roomInfo)
    {
        if (roomListItems.TryGetValue(roomInfo.Name, out GameObject roomListItemGO))
        {
            RoomDetails roomListItem = roomListItemGO.GetComponent<RoomDetails>();
            if (roomListItem != null)
            {
                roomListItem.AssignDetails(roomInfo.Name, roomInfo);
            }
        }
    }

    void ClearRoomListUI()
    {
        foreach (var item in roomListItems.Values)
        {
            Destroy(item);
        }
        roomListItems.Clear();
    }
    public void OnStopWaiting() 
    {
        EventHandler.LeaveRoom?.Invoke();
    }

  
}
