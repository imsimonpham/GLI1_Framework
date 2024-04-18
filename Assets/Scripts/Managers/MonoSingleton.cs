using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T:MonoSingleton<T>
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if(_instance == null)
                Debug.LogError(typeof(T).ToString() + " is null");
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = (T)this;
    }
}
