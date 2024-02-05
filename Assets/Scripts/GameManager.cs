using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;

    private byte[] _connectionToken;

    public Vector2 cameraViewRotation = Vector2.zero;
    public string playerNickname = "";

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else if(Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if(_connectionToken == null)
        {
            _connectionToken = ConnectionTokenUtils.NewToken();
        }
    }

    public void SetConnectionToken(byte[] token)
    {
        _connectionToken = token;
    }

    public byte[] GetConnectionToken()
    {
        return _connectionToken;
    }
}
