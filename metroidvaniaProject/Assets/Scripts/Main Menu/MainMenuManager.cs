using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    
    public void StartGame()
    {
        SceneLoader.Instance.LoadScene(SceneLoader.Instance.SceneHolder.levelOneName);
    }

    public void LoadSecondLevel()
    {
        SceneLoader.Instance.LoadScene(SceneLoader.Instance.SceneHolder.levelTwoName);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            LoadSecondLevel();
        }
    }

    public void QuitGame()
    {
        SceneLoader.Instance.QuitGame();
    }

}
