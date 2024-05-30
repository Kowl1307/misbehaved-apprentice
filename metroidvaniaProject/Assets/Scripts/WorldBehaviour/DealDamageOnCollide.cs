using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class DealDamageOnCollide : MonoBehaviour
{
    [SerializeField] private int damageAmount = 1;
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleEnter(collision.collider);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleEnter(other);
    }

    private void HandleEnter(Collider2D other)
    {
        //Don't deal damage to slime
        if (other.GetComponent<SlimeMovement>() != null)
            return;
            
        Debug.Log(other.transform.name);
        if (other.GetComponentInParent<PlayerStats>() != null)
        {
            PlayerStats.instance.DealDamage(damageAmount,0);
        }
    }
}
