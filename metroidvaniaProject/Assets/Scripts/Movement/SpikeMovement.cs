using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeMovement : MonoBehaviour, ITransformation
{
    public Rigidbody2D spikeRb;
    private BoxCollider2D coll;

    [SerializeField] private LayerMask walls;
    [SerializeField] private LayerMask slideObject;
    [SerializeField] private LayerMask pendulum;
    [SerializeField] private float swingMultiplier = 3;
    [SerializeField] private float rbWeight = 5;

    private float rotationAngle;
    private bool inPendulum = false;
    private Vector2 lastPositionInPendulum = Vector2.zero;
    private bool spikeHandler = true;
    private float staticY;
    private bool isHookedToSlider = false;
    //private bool inPendulum = false;

    private float originalGravityScale;
    //[SerializeField] private DissolveEffect dissolveEffect;

    //knock back variables
    public float KBForceSpike;
    public float KBCounterSpike;
    public float KBTotalTimeSpike;
    public bool KnockFromRightSpike;

    private PlayerController _playerController;
    private bool jumpOnSlider;
    private float slideTimer = 0.1f;
    private float slideCounter = 0.1f;

    private void Awake()
    {
        KBForceSpike = 8;
        KBTotalTimeSpike = 0.3f;
        spikeRb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        spikeRb.constraints = RigidbodyConstraints2D.FreezeRotation;
        spikeRb.mass = rbWeight;
        originalGravityScale = spikeRb.gravityScale;

        _playerController = GetComponentInParent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        //Gets input from "Horizontal axis", between -1 and 1 (left to right)
        //Debug.Log("angle before Going Down is called: " + GetAngle(rb.velocity).ToString());
        if (IsGoingDown(GetAngle(spikeRb.velocity)))
        {
            //Debug.Log("Going Down ");
            PointDownn();
        }
        //Debug.Log("velocity: " + KBForceSpike.ToString());
        if(KBCounterSpike > 0)
        {
            spikeRb.gravityScale = 7;
            spikeRb.constraints = RigidbodyConstraints2D.None;
            if (KnockFromRightSpike == true)
            {
                spikeRb.velocity = new Vector2(-KBForceSpike, KBForceSpike);
            }

            if (KnockFromRightSpike == false)
            {
                spikeRb.velocity = new Vector2(KBForceSpike, KBForceSpike);
            }

            KBCounterSpike -= Time.deltaTime;
        }
        jumpOnSlider = Input.GetAxis("Jump") > .1f;
    }

    private void FixedUpdate()
    {
        if (inPendulum)
        {
            SetLastPositionInPendulum();
        }
        
        JumpOnSlider();
    }

    private void SetLastPositionInPendulum()
    {
            IEnumerator SetNextFrame()
            {
                var currentPosition = transform.position;
                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();
                lastPositionInPendulum = currentPosition;
            }
            StartCoroutine(SetNextFrame());
    }

    //get Angle of human momentum
    private float GetAngle(Vector2 vel)
    {
        if (vel.x > 0)
        {
            return Mathf.Atan(vel.y / vel.x) * (180 / Mathf.PI);
        }
        else if (vel.x < 0)
        {
            return Mathf.Atan(vel.y / vel.x) * (180 / Mathf.PI) + 180;
        }
        else if (vel.x == 0 && vel.y > 0)
        {
            return 90f;
        }
        else if (vel.x == 0 && vel.y < 0)
        {
            return -90f;
        }
        return Mathf.Atan(vel.y / vel.x) * (180 / Mathf.PI);
    }

    //checks in which angle to handle colissionn
    private bool CheckCollisionAngle(LayerMask layer, float angle)
    {
        if (angle <= 90.01 && angle >= 89.99f)
        {
            return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, angle, Vector2.right, 0.1f, layer);
        }

        else if (angle <= 90.01 && angle >= 89.99f)
        {
            return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, angle, Vector2.left, 0.1f, layer);
        }

        else if (angle <= 90.01 && angle >= 89.99f)
        {
            return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, angle, Vector2.up, 0.1f, layer);
        }

        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, angle, Vector2.down, 0.1f, layer);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var wall = collision.collider;
        if ((1<<wall.gameObject.layer & walls.value) > 0)
        {

            //We hit something where we want to get stuck at
            spikeRb.constraints = RigidbodyConstraints2D.FreezePosition;
            
            
        }else if ((1<<wall.gameObject.layer & slideObject.value) > 0)
        {
            //We hit a slider

            //rb.velocity = Vector3.zero;
            //if (spikeHandler)
            //{
            //    staticY = transform.position.y;
            //}
            //spikeRb.velocity = transform.right * -5f;
            //spikeRb.velocity = transform.right * -10f;
            spikeRb.velocity = Vector2.right * 10f;
            spikeRb.gravityScale = 0;
            isHookedToSlider = true;

            //HandleWalking();
            //spikeHandler = false;
        }
        else if ((1 << wall.gameObject.layer & pendulum.value) > 0)
        {
            //We hit a pendulum
            //Debug.Log("rotation: "+collision.transform.eulerAngles.ToString());
         
            spikeRb.constraints = RigidbodyConstraints2D.FreezePosition;
            spikeRb.transform.parent = collision.gameObject.transform;

            inPendulum = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        spikeRb.gravityScale = originalGravityScale;

        var wall = collision.collider;

        if ((1 << wall.gameObject.layer & slideObject.value) > 0)
        {
            isHookedToSlider = false;
        }


    }

    private void PointDownn()
    {

        spikeRb.transform.rotation = Quaternion.Euler(0, 0, 0);

    }

    private bool IsGoingDown(float velocity)
    {
        if ((GetAngle(spikeRb.velocity) > 200f ||(GetAngle(spikeRb.velocity) < -20 && GetAngle(spikeRb.velocity) >= -90)) && spikeRb.velocity.y < -5.0)
        {
            return true;
        }
        return false;
    }

    public void InitTransformation()
    {
        //choose which directionn spike needs to turn
        //set momentum of spike when spawning
        //Debug.Log("Rotation angle: " + GetAngle(rb.velocity).ToString());
        //turn right
        if (Mathf.Abs(GetAngle(spikeRb.velocity)) < 80f && spikeRb.velocity.x > 0.2)
        {
            rotationAngle = 90; ;
        }
        //turn left
        else if (GetAngle(spikeRb.velocity) > 100f && GetAngle(spikeRb.velocity) < 260f && spikeRb.velocity.x < -0.2)
        {
            rotationAngle = -90;
        }
        //turn up
        else if (GetAngle(spikeRb.velocity) < 100f && GetAngle(spikeRb.velocity) > 80f && spikeRb.velocity.y > 0.4)
        {
            rotationAngle = 180;
        }
        //turn down
        else 
        {
            rotationAngle = 0;
        }


        spikeRb.transform.rotation = Quaternion.Euler(0, 0, rotationAngle);
        spikeRb.constraints = RigidbodyConstraints2D.FreezeRotation;

        inPendulum = false;
        lastPositionInPendulum = Vector2.zero;
        //dissolveEffect.Reappear(3f);
    }

    public void OnLeaveTransformation()
    {
        //unfreeze (if stuck)
        spikeRb.constraints = RigidbodyConstraints2D.None;
        spikeRb.constraints = RigidbodyConstraints2D.FreezeRotation;

        //If currently attached to a pendulum, we have to set the velocity of the rb so it can be grabbed by the transformation function
        if (inPendulum)
        {
            spikeRb.velocity = GetVelocityInPendulum();
            Debug.Log(GetVelocityInPendulum());
            //reappend to player controller if spike was made child of pendulum
            spikeRb.transform.parent = _playerController.transform;
            Debug.Log("Pendulum velocity: " + spikeRb.velocity);
        }
    }

    public Vector2 GetVelocityInPendulum()
    {
        //If currently attached to a pendulum, we have to set the velocity of the rb so it can be grabbed by the transformation function
        if (inPendulum)
        {
            Debug.Log("Pendulum Calculation: " + transform.position + "-" + lastPositionInPendulum);
            return (transform.position - (Vector3)lastPositionInPendulum) / Time.fixedDeltaTime * swingMultiplier;
        }
        return Vector3.zero;
    }

    public bool GetIsHookedToSlider()
    {
        return isHookedToSlider;
    }

    private void  JumpOnSlider()
    {
        if(slideCounter <=0)
        {
            //Debug.Log("jumping button:" + jumpOnSlider.ToString());
            if (isHookedToSlider && jumpOnSlider)
            {
                if (Math.Abs(transform.eulerAngles.z) < 0.1)
                {
                    spikeRb.AddForce(new Vector2(0, 20), ForceMode2D.Impulse);
                    spikeRb.transform.rotation = Quaternion.Euler(0, 0, 180);

                }
                else
                {
                    spikeRb.AddForce(new Vector2(0, -20), ForceMode2D.Impulse);
                    spikeRb.transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                isHookedToSlider = false;
            }
            slideCounter = slideTimer;
        }
        else if(isHookedToSlider)
        {
            slideCounter -= Time.deltaTime;
        }
        
    }
}
