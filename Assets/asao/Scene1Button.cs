using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
public class Scene1Button : MonoBehaviour
{
    public void OnClick()
    {
        SceneManager.LoadScene("MainGame1");
    }
}
