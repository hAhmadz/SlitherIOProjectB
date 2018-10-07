﻿using UnityEngine;

public class MenuController : MonoBehaviour
{
    //both play the same game at the moment from two different functions
    public void singlePlayerBtn()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void multiPlayerBtn()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1);
    }
}