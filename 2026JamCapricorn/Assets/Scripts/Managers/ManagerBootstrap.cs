using UnityEngine;
using UnityEngine.SceneManagement;

public static class ManagerBootstrap
{
    private const string ManagersScene = "Managers";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Load()
    {
        if (!SceneManager.GetSceneByName(ManagersScene).isLoaded)
            SceneManager.LoadScene(ManagersScene, LoadSceneMode.Additive);
    }
}
