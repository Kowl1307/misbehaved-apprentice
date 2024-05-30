using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SkeletonShieldBehaviour : MonoBehaviour
{
    
    private float movementSpeed = 3.0f;
    [SerializeField] private bool bIsGoingRight = true;
    private readonly float mRaycastingDistance = 1f;
    public PlayerStats playerStats;
    private Animator animator;
    private SpriteRenderer _mSpriteRenderer;
    private int health = 2;
    private BoxCollider2D coll;
    private Rigidbody2D rb;

    
    private HumanMovement humanMovement;
    private BallMovement ballMovement;
    private SpikeMovement spikeMovement;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        _mSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        playerStats = FindObjectOfType<PlayerStats>();
        animator = GetComponent<Animator>();
        coll = GetComponent<BoxCollider2D>();


        humanMovement = FindObjectOfType<HumanMovement>();
        ballMovement = FindObjectOfType<BallMovement>();
        spikeMovement = FindObjectOfType<SpikeMovement>();
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
        //Debug.Log("player position: " + humanMovement.transform.position.x.ToString()+"   enemy position "+this.transform.position.x.ToString());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Human":
                HumanCollisionSkeleton(collision);
                break;
            case "Ball":
                BallCollisionSkeleton(collision);
                break;
            case "Spike":
                SpikeCollisionSkeleton(collision);
                break;
            case "Slime":
                FlipSprite();
                break;
            case "Enemy":
                FlipSprite();
                break;
            default:
                Debug.Log("Collision with " + collision.gameObject.tag.ToString() + " not defined");
                break;
        }

        if (collision.gameObject.tag == "Enemy")
        {
            FlipSprite();
        }
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

    private void FlipSprite()
    {
        bIsGoingRight = !bIsGoingRight;
        _mSpriteRenderer.flipX = !bIsGoingRight;
    }

    public void HumanCollisionSkeleton(Collision2D collision)
    {
        humanMovement.KBCounterHuman = humanMovement.KBTotalTimeHuman;
        


        if (humanMovement.transform.position.x <= this.transform.position.x)
        {
            humanMovement.KnockFromRightHuman = true;
            if (bIsGoingRight)
            {
                bIsGoingRight = !bIsGoingRight;
                _mSpriteRenderer.flipX = !bIsGoingRight;
            }
        }

        if (humanMovement.transform.position.x >= this.transform.position.x)
        {
            humanMovement.KnockFromRightHuman = false;
            if (!bIsGoingRight)
            {
                bIsGoingRight = !bIsGoingRight;
                _mSpriteRenderer.flipX = !bIsGoingRight;
            }
        }
        if (health == 2) { animator.SetTrigger("humanAttack"); }
        if (health == 1) { animator.SetTrigger("skeletonAttack"); }
        playerStats.DealDamage(1);
        
    }

    public void BallCollisionSkeleton(Collision2D collision)
    {
        //handle collision with ball (first form)
        
        if(health == 2)
        {
            ballMovement.kbCounterBall = ballMovement.kbTotalTimeBall;

            if (ballMovement.transform.position.x <= this.transform.position.x)
            {
                ballMovement.knockFromRightBall = true;
                if (bIsGoingRight)
                {
                    bIsGoingRight = !bIsGoingRight;
                    _mSpriteRenderer.flipX = !bIsGoingRight;
                }
            }

            if (ballMovement.transform.position.x >= this.transform.position.x)
            {
                ballMovement.knockFromRightBall = false;
                if (!bIsGoingRight)
                {
                    bIsGoingRight = !bIsGoingRight;
                    _mSpriteRenderer.flipX = !bIsGoingRight;
                }
            }


            animator.SetBool("shieldAttack", false);
            animator.SetTrigger("ballAttack");
            health -= 1;
        }

        //second form
        else
        {
            ballMovement.kbCounterBall = ballMovement.kbTotalTimeBall;

            if (ballMovement.transform.position.x <= this.transform.position.x)
            {
                ballMovement.knockFromRightBall = true;
                if (bIsGoingRight)
                {
                    bIsGoingRight = !bIsGoingRight;
                    _mSpriteRenderer.flipX = !bIsGoingRight;
                }
            }

            if (ballMovement.transform.position.x >= this.transform.position.x)
            {
                ballMovement.knockFromRightBall = false;
                if (!bIsGoingRight)
                {
                    bIsGoingRight = !bIsGoingRight;
                    _mSpriteRenderer.flipX = !bIsGoingRight;
                }
            }

            animator.SetTrigger("skeletonAttack");
            playerStats.DealDamage(1);
        }
    }

    public void SpikeCollisionSkeleton(Collision2D collision)
    {
        //first form
        if(health==2)
        {
            spikeMovement.KBCounterSpike = spikeMovement.KBTotalTimeSpike;
            
            if (spikeMovement.transform.position.x <= this.transform.position.x)
            {
                spikeMovement.KnockFromRightSpike = true;
                if (bIsGoingRight)
                {
                    bIsGoingRight = !bIsGoingRight;
                    _mSpriteRenderer.flipX = !bIsGoingRight;
                }
            }

            if (spikeMovement.transform.position.x >= this.transform.position.x)
            {
                spikeMovement.KnockFromRightSpike = false;
                if (!bIsGoingRight)
                {
                    bIsGoingRight = !bIsGoingRight;
                    _mSpriteRenderer.flipX = !bIsGoingRight;
                }
            }

            
            animator.SetBool("shieldAttack", true);
            animator.SetTrigger("ballAttack");
            playerStats.DealDamage(1);
        }
        else
        {
            
            health -= 1;
            animator.SetTrigger("spikeAttack");
            this.gameObject.layer = LayerMask.NameToLayer("DeadEnemy");
            
            movementSpeed = 0;
            rb.constraints = RigidbodyConstraints2D.FreezePosition;
            Destroy(rb);
            
        }
    }

}
