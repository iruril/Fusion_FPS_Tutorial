using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InGameMessegedUIHandler : MonoBehaviour
{
    public TextMeshProUGUI[] textMeshProUGUIs;

    Queue messegeQueue = new Queue();

    void Start()
    {
        
    }

    public void OnGameMessegeRecieved(string messege)
    {
        messegeQueue.Enqueue(messege);

        if(messegeQueue.Count > 3) messegeQueue.Dequeue();

        int queueIndex = 0;
        foreach(string item in messegeQueue)
        {
            textMeshProUGUIs[queueIndex].text = item;
            queueIndex++;
        }
    }
}
