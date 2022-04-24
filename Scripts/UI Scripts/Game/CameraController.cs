using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraAngel
{
    menu = 0,
    whiteTeam = 1,
    blackTeam = 2,
}
public class CameraController : MonoBehaviour
{
    public static CameraController Instance { set; get; }
    [SerializeField] private GameObject[] cameraAngles;

    public void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    // Cameras
    public void ChangeCamera(CameraAngel index)
    {
        for (int i = 0; i < cameraAngles.Length; i++)
        {
            cameraAngles[i].SetActive(false);
        }
        cameraAngles[(int)index].SetActive(true);
    }

}
