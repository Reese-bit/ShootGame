using UnityEngine;

public class PersistenSingleton<T> : MonoBehaviour where T : Component
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        
        //DontDestroyOnLoad only works for root GameObjects or components on root GameObjects.
        //It means in the hierarchy, the game object with the code "DontDestroyOnLoad" must be the 'root'
        //(ie: not a child game object).
        DontDestroyOnLoad(gameObject);
    }
}
