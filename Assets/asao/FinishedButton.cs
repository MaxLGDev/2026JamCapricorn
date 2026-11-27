using UnityEngine;
using UnityEngine.SceneManagement;
public class FinishedButton : MonoBehaviour
{
    public void Onclicked()
    {
        SceneManager.LoadScene("TittleScene");
    }
}
