using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    [SerializeField] private Transform loadingScreenParent;
    [SerializeField] private Slider loadingProgressBar;

    private AsyncOperation _loadingOperation;
    private bool _showingLoadingScreen = false;
    
    public SceneHolder SceneHolder { get; private set; }

    private void Awake()
    {
        Instance = this;
        SceneHolder = Resources.Load<SceneHolder>("SceneHolder");
    }


    private void Start()
    {
        StartCoroutine(StartPanicButton());
    }

    private void Update()
    {
        if (!_showingLoadingScreen)
            return;
        
        loadingProgressBar.value = _loadingOperation.progress;
    }
    
    private IEnumerator StartPanicButton()
    {
        while (true)
        {
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));

            SceneLoader.Instance.LoadMainMenu();
        }
    }

    public void LoadScene(string name)
    {
        ShowLoadingScreen(true);
        _loadingOperation = SceneManager.LoadSceneAsync(name);
        _loadingOperation.completed += _ =>
        {
            ShowLoadingScreen(false);
            _loadingOperation = null;
        };
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ShowLoadingScreen(bool show)
    {
        _showingLoadingScreen = show;

        loadingProgressBar.value = 0;
            
        loadingScreenParent.gameObject.SetActive(_showingLoadingScreen);
    }

    public void LoadMainMenu()
    {
        LoadScene(SceneHolder.mainMenuName);
    }
}
