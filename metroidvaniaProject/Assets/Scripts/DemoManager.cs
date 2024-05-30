using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoManager : MonoBehaviour
{
    public void OnEnable()
    {
        PlayerStats.instance.onDeath.AddListener(OnDeath);
    }

    public void OnDisable()
    {
        PlayerStats.instance.onDeath.RemoveListener(OnDeath);
    }

    public void OnDeath()
    {
        StartCoroutine(ReturnToMainMenuOnClick());
    }

    private IEnumerator ReturnToMainMenuOnClick()
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Escape));

        SceneLoader.Instance.LoadMainMenu();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            FindObjectOfType<PlayerStats>().Heal(1);
        }
    }
}
