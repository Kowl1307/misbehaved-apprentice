using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkeletonBehaviour : MonoBehaviour
{
    
    [SerializeField] private LayerMask walls;
    private PlayerController _Player_Controller;
    private  float movementSpeed = 3.0f;
    private bool bIsGoingRight = true;
    private float mRaycastingDistance = 1f;
    private BoxCollider2D coll;
    private PlayerStats playerStats;
    private Animator animator;

    private SpriteRenderer _mSpriteRenderer;

    private HumanMovement humanMovement;
    private BallMovement ballMovement;
    private SpikeMovement spikeMovement;

    private float velocityThresholdForKilling = .1f;


    private void Awake()
    {
        _Player_Controller = FindObjectOfType<PlayerController>();
        _mSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        humanMovement = FindObjectOfType<HumanMovement>();
        ballMovement = FindObjectOfType<BallMovement>();
        playerStats = FindObjectOfType<PlayerStats>();
        spikeMovement = FindObjectOfType<SpikeMovement>();
        
        coll = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        _mSpriteRenderer.flipX = bIsGoingRight;
    }

    // Update is called once per frame
    void Update()
    {
        
        // if the ennemy is going right, get the vector pointing to its right
        Vector3 directionTranslation = (bIsGoingRight) ? transform.right : -transform.right;
        directionTranslation *= Time.deltaTime * movementSpeed;
        transform.Translate(directionTranslation);

        CheckForWalls();
        CheckForCliffs();
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        //Collision with player
        switch (collision.gameObject.tag)
        {
            case "Human":
                HumanCollisionSkeleton(collision);
                break;
            case "Ball":
                BallCollisionSkeleton(collision);
                break;
            case "Spike":
                SpikeCollisionSkeleton();
                break;
            case "Slime":
                FlipSprite();
                break;
            case "Enemy":
                FlipSprite();
                break;
            default:
                Debug.Log("Collision with " +collision.gameObject.tag.ToString()+ " not defined");
                break;
        }

        /*
        //Temporary Fix for beta
        if (collision.gameObject.CompareTag("walls"))
        {
            Debug.Log("i was entered");
            bIsGoingRight = !bIsGoingRight;
            _mSpriteRenderer.flipX = !bIsGoingRight;
        }
        */
    }
    

    
    private void CheckForWalls()
    {
        Vector3 raycastDirection = (bIsGoingRight) ? Vector3.right : Vector3.left;

        // Raycasting takes as parameters a Vector3 which is the point of origin, another Vector3 which gives the direction, and finally a float for the maximum distance of the raycast
        RaycastHit2D[] hitsDown = Physics2D.RaycastAll(transform.position + raycastDirection * mRaycastingDistance - new Vector3(0f, 1.25f, 0f), raycastDirection, 0.1f);
        RaycastHit2D[] hitsMid = Physics2D.RaycastAll(transform.position + raycastDirection * mRaycastingDistance , raycastDirection, 0.1f);
        RaycastHit2D[] hitsUp = Physics2D.RaycastAll(transform.position + raycastDirection * mRaycastingDistance + new Vector3(0f, 1.25f, 0f), raycastDirection, 0.1f);
        // if we hit something, check its tag and act accordingly
       
        if (hitsDown.Length > 0 && hitsDown.ToList().Exists(hit => hit.transform.CompareTag("walls")) ||
            hitsMid.Length > 0 && hitsMid.ToList().Exists(hit => hit.transform.CompareTag("walls")) ||
        hitsUp.Length > 0 && hitsUp.ToList().Exists(hit => hit.transform.CompareTag("walls")))
        {
            FlipSprite();
        }

    }

    private void CheckForCliffs()
    {
        Vector3 raycastDirection = (bIsGoingRight) ? Vector3.right : Vector3.left;

        // Raycasting takes as parameters a Vector3 which is the point of origin, another Vector3 which gives the direction, and finally a float for the maximum distance of the raycast
        RaycastHit2D hit = Physics2D.Raycast(transform.position + mRaycastingDistance * raycastDirection, Vector3.down, 2f);

        // if we don't hit something
        if (hit.collider == null)
        {

            FlipSprite();
        }
    }

    public void HumanCollisionSkeleton(Collision2D collision)
    {
        //if (collision.gameObject.tag == "Human")
    
        //Reset knockback time
        humanMovement.KBCounterHuman = humanMovement.KBTotalTimeHuman;


        //handle  position of skeleton

        //attack human from right
        if (humanMovement.transform.position.x <= this.transform.position.x)
        {
            humanMovement.KnockFromRightHuman = true;
            //if going right rotate sprite
            if (bIsGoingRight)
            {
                FlipSprite();
            }
        }

        //attack human from left
        if (humanMovement.transform.position.x >= this.transform.position.x)
        {
            humanMovement.KnockFromRightHuman = false;
            //if going left rotate sprite
            if (!bIsGoingRight)
            {
                FlipSprite();
            }
        }

            
        animator.SetTrigger("atackTrigger");
        playerStats.DealDamage(1);
    }

    public void BallCollisionSkeleton(Collision2D collision)
    {
        //collision with ball
        //else if (collision.gameObject.tag == "Ball")

        //Reset knockback time
        ballMovement.kbCounterBall = ballMovement.kbTotalTimeBall;

        if (ballMovement.transform.position.x <= this.transform.position.x)
        {
            ballMovement.knockFromRightBall = true;
            if (bIsGoingRight)
            {
                FlipSprite();
            }
        }

        if (ballMovement.transform.position.x >= this.transform.position.x)
        {
            ballMovement.knockFromRightBall = false;
            if (!bIsGoingRight)
            {
                FlipSprite();
            }
        }
        animator.SetTrigger("atackTrigger");
        playerStats.DealDamage(1);
    }

    public void SpikeCollisionSkeleton ()
    {  
        //check the velocity on y axis, once the spike is not down while collision with skeleton, perform transform back to human form
        if (spikeMovement.spikeRb.velocity.magnitude > velocityThresholdForKilling)
        {
            animator.SetTrigger("death");
            this.gameObject.layer = LayerMask.NameToLayer("DeadEnemy");
            movementSpeed = 0;
        }
        else
        {
            _Player_Controller.PerformTransformation(PlayerController.TransformationType.Human);
        }
    }

    private void FlipSprite()
    {
        bIsGoingRight = !bIsGoingRight;
        _mSpriteRenderer.flipX = !bIsGoingRight;
    }


}
