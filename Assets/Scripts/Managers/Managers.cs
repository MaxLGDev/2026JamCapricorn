using UnityEngine;

public abstract class Managers<T> : MonoBehaviour where T : Managers<T>
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = (T)this;
    }
}
