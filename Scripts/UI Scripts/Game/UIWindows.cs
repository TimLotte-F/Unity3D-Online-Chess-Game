using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWindows : MonoBehaviour
{
    public void HiddenWindow()
    {
        this.gameObject.SetActive(false);
    }
    public void ShowWindow()
    {
        this.gameObject.SetActive(true);
    }


}
