using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public class OptionSceneButton : MonoBehaviour
{
    public void OnClicked()
    {
        SceneManager.LoadScene("OptionScene");
    }
}
