using UnityEngine;

public class MenuController : MonoBehaviour
{
    //both play the same game at the moment from two different functions
    public void singlePlayerBtn()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Scenes/JoystickScene");
    }

    public void multiPlayerBtn()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Scenes/TouchScene");
    }

    public void accelerometerBtn()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Scenes/AccelerometerScene");
    }
}
