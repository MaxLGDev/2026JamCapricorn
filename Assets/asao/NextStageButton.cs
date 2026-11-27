using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
public class NextStageButton : MonoBehaviour
{
    public void OnClicked()
    {
        SceneManager.LoadScene("MainGame1");
    }
}
