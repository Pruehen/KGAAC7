using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonComponent<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<T>();
                Debug.Assert(_instance != null,
                    $"There is no {nameof(T)} in the scene");
            }
            return _instance;
        }
    }


}
