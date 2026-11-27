using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
public class FinishButton : MonoBehaviour
{
    public void OnClicked()
    {
        SceneManager.LoadScene("MainGame2");
    }
}
