using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System;
using UnityEngine.SceneManagement;

public class MainMenuUIHandler : MonoBehaviour
{
    public TMP_InputField InputField;

    void Start()
    {
        if (PlayerPrefs.HasKey("PlayerNickname"))
        {
            InputField.text = PlayerPrefs.GetString("PlayerNickname");
        }
        
    }

    public void OnJoinedGameClicked()
    {
        string nickName = GetRemoveWhiteSpaces(InputField.text);
        if (nickName != string.Empty)
        {
            PlayerPrefs.SetString("PlayerNickname", InputField.text);
        }
        else
        {
            PlayerPrefs.SetString("PlayerNickname", "Idiot");
        }
        PlayerPrefs.Save();

        SceneManager.LoadScene("MainScene");
    }

    public string GetRemoveWhiteSpaces(string str)
    {
        return string.Concat(str.Where(c => !Char.IsWhiteSpace(c)));
    }
}
