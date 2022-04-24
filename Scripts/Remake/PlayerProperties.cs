using ExitGames.Client.Photon;
public class PlayerProperties
{
    public const string Team = "Team";
    public const string TeamNumber = "TmNo";
    public const string isReady = "R";
    public const string Score = "Score";

    public static Hashtable getPlayerProperties()
    {
        Hashtable playerProperties = new Hashtable();
        playerProperties.Add(Team, null);
        playerProperties.Add(TeamNumber, null);
        playerProperties.Add(isReady, false);
        playerProperties.Add(Score, 0);
        
        return playerProperties;
    }
}
