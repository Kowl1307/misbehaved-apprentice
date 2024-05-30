using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats instance;

    public UnityEvent<int> onHealthChange = new UnityEvent<int>();
    public UnityEvent onDeath = new UnityEvent();

    //check for invulnability
    public bool inv = false;
    public float absorbStart;
    public float absorbDur;
    public float flameabsorb;


    /// <summary>
    /// Maximal health in hearts
    /// </summary>
    public int MaxHealth => maxHealth;

    [SerializeField] private int maxHealth = 3;

    public int CurrentHealth => currentHealth;
    
    [SerializeField] private int currentHealth = 3;


    private void Awake()
    {
        instance = this;
    }

    public void DealDamage(int damageAmount, int dmgType = 0) //dmgType 0= spike, 1=fire
    {

        bool flamecheck1 = ((flameabsorb + 10f) > Time.time) ;
        bool flamecheck2 = flamecheck1 && (dmgType == 1);
        if (!inv && !flamecheck2) {
            currentHealth -= damageAmount;
            onHealthChange.Invoke(-damageAmount);
        }
        if (currentHealth <= 0)
        {
            onDeath.Invoke();
        }
    }

    public void Heal(int healAmount)
    {
        currentHealth = Mathf.Clamp(currentHealth+healAmount,0,maxHealth);
        onHealthChange.Invoke(healAmount);
    }
    
}
