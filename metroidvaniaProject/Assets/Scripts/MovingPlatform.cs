using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private int direction = 1;
    [SerializeField] private float speed = 10;
    [SerializeField] private float maxDistance = 7;
    private float distance;
    private Vector3 startPosition;

    private Transform playerParent;
    
    private void Awake()
    {
        playerParent = FindObjectOfType<PlayerController>().transform;
    }

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
        distance = (startPosition - transform.position).magnitude;

        Debug.Log("Distance: " + distance);

        if(distance > maxDistance)
        {
            direction *= -1;
        }
    }

    private Vector3 CurrenntMovementTarget()
    {
        //if moving to the right
        if (direction == 1)
        {
            return transform.position + Vector3.right;
        }

        return transform.position + Vector3.left;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        collision.transform.parent = transform;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.gameObject.activeSelf)
            return;
        
        if (collision.transform.TryGetComponent(typeof(ITransformation), out var comp))
        {
            collision.transform.parent = playerParent;
            return;
        }
        
        //TODO: This throws error on Disable/Destroy
        collision.transform.SetParent(null);
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        var position = transform.position;
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(position, position + Vector3.right * direction * maxDistance);
    }
    #endif
}
