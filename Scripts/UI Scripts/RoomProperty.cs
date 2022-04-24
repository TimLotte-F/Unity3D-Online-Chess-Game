using ExitGames.Client.Photon;
using Photon.Realtime;

public class RoomProperty
{
    public const string Owner = "P1";
    public const string Player2 = "P2";
    public const string RoundTime = "RT";
    public const int MAX_PLAYERS = 2;
    public const string maxPlayer = "MP";
    public const int EmptyRoomTtL = 0;// 1,000 = 1s 
    public const int PlayerTtL = 300000;

    public static Hashtable getRoomProperties()
    {
        Hashtable properties = new Hashtable();
        properties.Add(RoundTime, null);
        properties.Add(Owner, null);
        properties.Add(Player2, null);

        return properties;
    }
    public static RoomOptions getRoomOptions(string OwnerName, int roundTime)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.CustomRoomProperties = getRoomProperties();
        roomOptions.CustomRoomProperties[Owner] = OwnerName;
        roomOptions.CustomRoomProperties[RoundTime] = roundTime;

        roomOptions.MaxPlayers = MAX_PLAYERS;
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;

        // for rejoined, reconnected, and AFK checking.
        roomOptions.CleanupCacheOnLeave = true;
        roomOptions.EmptyRoomTtl = EmptyRoomTtL;
        roomOptions.PlayerTtl = PlayerTtL; // 60,000 ms = 1 min, For AKF checking.

        // RoundTime can be got by lobby
        roomOptions.CustomRoomPropertiesForLobby = new string[] {
            RoundTime,
            Owner,
            Player2
        };
        return roomOptions;
    }

    public static string getPlayerNameInRoom(RoomInfo room, string property)
    {
        object playerName = room.CustomProperties[property];
        return playerName != null ? playerName.ToString() : "Empty";
    }
}
