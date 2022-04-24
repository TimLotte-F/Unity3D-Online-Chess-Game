using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class RoomListing : MonoBehaviourPun                        // Without any mistake checked by Tim.
{
    
    public static RoomListing Instance;
    private List<RoomInfo> roomlisting;
    public List<RoomInfo> GetRoomlisting()
    {
        return roomlisting;
    }

    public void SetRoomlisting(List<RoomInfo> value)
    {
        roomlisting = value;
    }

    private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();
    private void Start()
    {
        if (Instance == null)
            Instance = this;

        roomlisting = new List<RoomInfo>();
    }

    public void OnUpdatedRoomList(List<RoomInfo> roomList)
    {
        UpdateCachedRoomList(roomList); // cleanup the room which is empty or invisible or un-open room.
        UpdateRoomListView();
    }
    public void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
           // print(info.Name);
            if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
            {
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList.Remove(info.Name);
                }
                continue;
            }

            if (cachedRoomList.ContainsKey(info.Name))
            {
                cachedRoomList[info.Name] = info;
            }
            else
            {
                cachedRoomList.Add(info.Name, info);
            }
        }
    }

    public void UpdateRoomListView()
    {
        if (roomlisting != null)
            roomlisting.Clear();

        if (cachedRoomList != null)
            foreach (RoomInfo info in cachedRoomList.Values)
            {
                // Debug.Log("ïîâÆñº:" + info.Name + "êlêîÅF" + (byte)info.PlayerCount + "Max:" + info.MaxPlayers);
                roomlisting.Add(info);
            }
    }

    public bool isRoomNameRepeat(string roomName)
    {
        List<RoomInfo> roomInfos = roomlisting;
        foreach (RoomInfo info in roomInfos)
            if (roomName == info.Name)
                return true;
        return false;
    }
}
