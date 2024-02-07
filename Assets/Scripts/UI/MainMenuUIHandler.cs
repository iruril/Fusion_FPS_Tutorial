using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuUIHandler : MonoBehaviour
{
    public TMP_InputField InputField;
    public TMP_InputField sessionNameInputField;

    public GameObject playerDetails;
    public GameObject sessonBrowser;
    public GameObject createSession;
    public GameObject status;

    void Start()
    {
        if (PlayerPrefs.HasKey("PlayerNickname"))
        {
            InputField.text = PlayerPrefs.GetString("PlayerNickname");
        }
        
    }

    private void HideAllPanels()
    {
        playerDetails.SetActive(false);
        sessonBrowser.SetActive(false);
        createSession.SetActive(false);
        status.SetActive(false);
    }

    public void OnFindGameClicked()
    {
        string nickName = InputField.text;
        if (nickName != string.Empty)
        {
            PlayerPrefs.SetString("PlayerNickname", InputField.text);
        }
        else
        {
            PlayerPrefs.SetString("PlayerNickname", "Idiot");
        }
        PlayerPrefs.Save();

        GameManager.Instance.playerNickname = InputField.text;

        NetworkRunnerHandler networkRunnerHandler = FindObjectOfType<NetworkRunnerHandler>();
        networkRunnerHandler.OnJoinLobby();

        HideAllPanels();
        sessonBrowser.SetActive(true);

        FindObjectOfType<SessionListUIHandler>(true).OnLookingForSession();
    }

    public void OnCreateNewGameClicked()
    {
        HideAllPanels();
        createSession.SetActive(true);
    }

    public void OnStartNewSessionClicked()
    {
        NetworkRunnerHandler netRunnerHandler = FindObjectOfType<NetworkRunnerHandler>();

        netRunnerHandler.CreateSession(sessionNameInputField.text, "MainScene");
        HideAllPanels();
        status.SetActive(true);
    }

    public void OnJoiningSession()
    {
        HideAllPanels();
        status.SetActive(true);
    }
}
