using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;
using UnityEngine.UI;

public class SessionListUIHandler : MonoBehaviour
{
    public TextMeshProUGUI statusText;
    public GameObject sessionListItemPrefab;
    public VerticalLayoutGroup verticalLayoutGroup;

    private void Awake()
    {
        ClearList();
    }

    public void ClearList()
    {
        foreach(Transform item in verticalLayoutGroup.transform)
        {
            Destroy(item.gameObject);
        }

        statusText.gameObject.SetActive(false);
    }

    public void AddToList(SessionInfo sessionInfo)
    {
        SessionInfoListItem item = Instantiate(sessionListItemPrefab, verticalLayoutGroup.transform).GetComponent<SessionInfoListItem>();
        item.SetInformation(sessionInfo);

        item.OnJoinSession += OnJoinSessionTo;
    }

    private void OnJoinSessionTo(SessionInfo goal)
    {
        NetworkRunnerHandler networkRunnerHandler = FindObjectOfType<NetworkRunnerHandler>();
        networkRunnerHandler.JoinSession(goal);

        MainMenuUIHandler mainMenuUIHandler = FindObjectOfType<MainMenuUIHandler>();
        mainMenuUIHandler.OnJoiningSession();
    }

    public void OnFoundSessionFailed()
    {
        ClearList();

        statusText.text = "No Game Exists!";
        statusText.gameObject.SetActive(true);
    }

    public void OnLookingForSession()
    {
        ClearList();

        statusText.text = "Looking For Sessions...";
        statusText.gameObject.SetActive(true);
    }
}
