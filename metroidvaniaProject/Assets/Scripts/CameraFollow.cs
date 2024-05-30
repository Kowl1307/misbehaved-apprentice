using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] public float followSpeed = 2f;
    public float viewDistance = 15;
    [SerializeField] public Transform target;


    private void Awake()
    {
        if (target == null)
            target = FindObjectOfType<HumanMovement>().transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 newPos = new Vector3(target.position.x, target.position.y, -viewDistance);
        transform.position = Vector3.Slerp(transform.position, newPos, followSpeed * Time.deltaTime);
    }
   

  }

