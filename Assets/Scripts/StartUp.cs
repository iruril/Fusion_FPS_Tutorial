using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartUp
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void InitPrefabs()
    {
        GameObject[] prefabToInstantiate = Resources.LoadAll<GameObject>("InstantiateOnLoad/");

        foreach(var item in prefabToInstantiate)
        {
            GameObject.Instantiate(item);
        }
    }
}
