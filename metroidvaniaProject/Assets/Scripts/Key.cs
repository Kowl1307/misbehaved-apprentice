using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject key;
    public bool KeyObtained = false; // check if the key is getted
    [SerializeField] private GameObject _Slime;
    [SerializeField] private DissolveEffect dissolveEffect;
   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Slime")
        {
            //dissolveEffect.Disappear(1f);
            //FunctionTimer.Create(TeleportBack,1f); //Delay for a Sec
            //dissolveEffect.Reappear(1f);
            KeyObtained = true;
            key.SetActive(false);
        }
    }

    /*
    private void TeleportBack()
    {
        _Slime.transform.position = new Vector2(_Slime.transform.position.x, _Slime.transform.position.y + 12);
    }
    */
}
