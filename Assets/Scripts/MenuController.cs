using UnityEngine;

public class MenuController : MonoBehaviour
{
    //both play the same game at the moment from two different functions
    public void singlePlayerBtn()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Scenes/SinglePlayerScene");
    }

    public void multiPlayerBtn()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Scenes/MultiPlayerScene");
    }
}
