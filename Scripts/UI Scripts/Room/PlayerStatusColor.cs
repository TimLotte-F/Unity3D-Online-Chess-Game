using UnityEngine;

public class PlayerStatusInfo
{
    public const string READY = "(Ready)";
    public const string NOTREADY = "(Not Ready)";
    public const string ROOMMASTER = "(Room Master)";
    public const string START = "Start";

    public static Color notReadyColor = new Color32(255, 45, 0, 250);
    public static Color ReadyColor = new Color32(0, 255, 45, 250);
    public static Color WarningColor = new Color32(255, 245, 0, 255);
}
