using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DamageEffect : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void OnEnable()
    {
        PlayerStats.instance.onHealthChange.AddListener(OnHealthChange);
    }

    public void OnDisable()
    {
        PlayerStats.instance.onHealthChange.RemoveListener(OnHealthChange);
    }

    public void OnHealthChange(int changeAmount)
    {
        if (changeAmount > 0)
            return;

        animator.SetTrigger("Damage");

        if (PlayerStats.instance.CurrentHealth <= 0)
            animator.SetTrigger("Death");
    }
     
}
