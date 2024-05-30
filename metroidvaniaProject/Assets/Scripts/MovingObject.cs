using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MovingObject : MonoBehaviour
{
    [SerializeField] private Vector2 direction = Vector2.right;
    [SerializeField] private float speed = 10;
    [SerializeField] private float maxDistance = 7;
    private float distance;
    private Vector3 startPosition;
    private bool turnedAround = false;
    
    private void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 target = CurrenntMovementTarget();
        transform.position = Vector3.Lerp(transform.position, target, speed * Time.deltaTime);

        //update direction
        distance = turnedAround
            ? (transform.position - (startPosition + (Vector3)direction * (-1 * maxDistance))).magnitude
            : (startPosition - transform.position).magnitude;
 
        //Debug.Log("Distance: " + distance);

        if (!(distance > maxDistance)) return;
        
        direction *= -1;
        turnedAround = !turnedAround;
    }

    private Vector3 CurrenntMovementTarget()
    {
        return transform.position + (Vector3)direction;
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        var position = transform.position;
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(position, position + (Vector3)direction * maxDistance);
    }
    #endif
}
