using System;
using UnityEngine;

//using UnityEditor;

[CreateAssetMenu(fileName = "SceneHolder", menuName = "SceneHolder")]
public class SceneHolder : ScriptableObject
{
    //public SceneAsset mainMenuScene;
    public string mainMenuName = "Main Menu";
    //public SceneAsset levelOneScene;
    public string levelOneName = "MainScene";

    public string levelTwoName = "Level2";

}