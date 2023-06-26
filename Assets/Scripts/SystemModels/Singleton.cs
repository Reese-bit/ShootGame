using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    public static T instance {get; private set;}

    protected virtual void Awake()
    {
        instance = this as T;
    }
}
