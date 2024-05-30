using System;
using System.Timers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Pipegates : MonoBehaviour
{

    private GameObject _player;
    private float TeleportTime = 1f;
    private DissolveEffect dissolveEffect;
    private Rigidbody2D rb;
    [SerializeField] private Transform destination;

    private void Awake()
    {
        _player = FindObjectOfType<SlimeMovement>().gameObject;
        dissolveEffect = FindObjectOfType<DissolveEffect>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        /*
        if(collision.gameObject.tag == "Slime")
        {
            dissolveEffect.Disappear(1f);

            FunctionTimer.Create( TeleportIntoPipe, TeleportTime);

            FunctionTimer.Create(ReappearingIntoPipe, 1f);
        }
        */
    }

    public void ActivatePipe()
    {
        dissolveEffect.Disappear(1f);

        FunctionTimer.Create(TeleportIntoPipe, TeleportTime);

        FunctionTimer.Create(ReappearingIntoPipe, 1f);
    }
    private void TeleportIntoPipe()
    {
        _player.transform.position = destination.position;
    }

    private void ReappearingIntoPipe()
    {
        dissolveEffect.Reappear(1f);
    }
}
