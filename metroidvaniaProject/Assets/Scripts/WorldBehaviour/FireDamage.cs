using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class FireDamage : MonoBehaviour
{
    [SerializeField] private int damageAmount = 3;
    private float fireabsorbed;

    public static float fireAbsorbDuration = 10f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.name);
        HandleCollision(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        HandleCollision(other);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleCollision(collision.collider);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        HandleCollision(collision.collider);
    }

    void HandleCollision(Collider2D collider)
    {
        var other = collider;
        var componentInParent = other.GetComponentInParent<PlayerStats>();
        
        if (other.transform.name == "Slime Form") {
            if ((componentInParent.absorbDur + componentInParent.absorbStart)> Time.fixedTime) { 
                //Debug.Log("Flame absorbed, immune to fire for 10sec");
                fireabsorbed = Time.fixedTime;
                componentInParent.flameabsorb = Time.fixedTime;
                other.transform.GetComponent<SlimeMovement>().AbsorbFire();
            }
        }

        if (componentInParent != null && (componentInParent.flameabsorb +fireAbsorbDuration) > Time.fixedTime)
        {
            PlayerStats.instance.DealDamage(damageAmount,1);
        }
    }
}
