using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Pendulum : MonoBehaviour
{
    [SerializeField] private bool alwaysShowGizmo = false;
    [SerializeField] private float startAngle = -70;
    [SerializeField] private SpriteRenderer chainRenderer;
    
    private float startTime = 0;

    private float interpolatedAngle = 0;
    private float gravityScale;
    private float pendulumLength;

    private void Awake()
    {
        gravityScale = Mathf.Abs(Physics2D.gravity.y);
        pendulumLength = chainRenderer.size.y;
    }

    private void Start()
    {
        startTime = Time.time;
    }

    private void OnValidate()
    {
        transform.rotation = Quaternion.Euler(0,0,startAngle);
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(alwaysShowGizmo || Selection.activeGameObject == gameObject) 
            Gizmos.DrawWireSphere(transform.position, transform.GetChild(0).GetComponent<SpriteRenderer>().size.y);
    }
    
    #endif
    private void FixedUpdate()
    {
        var omega = Mathf.Sqrt(gravityScale / pendulumLength);
        var theta = startAngle * Mathf.Cos(omega * (Time.time-startTime));
        
        
        transform.rotation = Quaternion.Euler(0, 0, theta);
    }
}
