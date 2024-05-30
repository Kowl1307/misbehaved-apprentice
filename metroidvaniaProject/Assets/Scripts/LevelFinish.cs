using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelFinish : MonoBehaviour
{
    [SerializeField] private Key toObtainKey;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.parent.CompareTag("Player"))
        {
            if(toObtainKey.KeyObtained)
            {
                EndLevel();
            }
        }
    }


    private void EndLevel()
    {
        GetComponent<Animator>().SetTrigger("Finish");

        StartCoroutine(ReturnToMainMenuOnClick());
    }

    private IEnumerator ReturnToMainMenuOnClick()
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Escape));

        SceneLoader.Instance.LoadMainMenu();
    }
}
