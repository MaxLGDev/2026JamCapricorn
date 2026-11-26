using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneInit : MonoBehaviour
{
    void Start() => SceneManager.SetActiveScene(gameObject.scene);
}
