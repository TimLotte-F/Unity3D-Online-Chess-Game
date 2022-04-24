
using TMPro;
using UnityEngine;

public class AuthUIManager : MonoBehaviour
{
    public static AuthUIManager Instance;
    [Header("References")]
    // [SerializeField] private GameObject checkingForAccountUI;
    [SerializeField] private GameObject loginUI;
    [SerializeField] private GameObject registerUI;
    // [SerializeField] private GameObject verifyEmailUI;
    // [SerializeField] private TMP_Text verifyEmailText;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoginScreen();
        }
        else if (Instance != this)
        {
            Destroy(this.gameObject);
        }

    }


    private void ClearUI()
    {
        registerUI.SetActive(false);
        loginUI.SetActive(false);
        if (FirebaseManager.Instance != null)
            FirebaseManager.Instance.ClearOutputs();
    }
    public void LoginScreen()
    {
        ClearUI();
        loginUI.SetActive(true);
    }
    public void ResgierScreen()
    {
        ClearUI();
        registerUI.SetActive(true);
    }

}
