using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimpleEnemyBehaviour : MonoBehaviour
{
    [SerializeField] private LayerMask walls;
    private readonly float movementSpeed = 3.0f;
    private bool bIsGoingRight = true;
    private float mRaycastingDistance = 1f;
    private BoxCollider2D coll;
    public PlayerStats playerStats;

    private SpriteRenderer _mSpriteRenderer;

    private HumanMovement humanMovement;

    private float velocityThresholdForKilling = .1f;
    
    // Start is called before the first frame update
    void Start()
    {
        _mSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        _mSpriteRenderer.flipX = bIsGoingRight;
        coll = GetComponent<BoxCollider2D>();
        playerStats = FindObjectOfType<PlayerStats>();

        humanMovement = FindObjectOfType<HumanMovement>();
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
        if(collision.gameObject.tag == "Human")
        {
            humanMovement.KBCounterHuman = humanMovement.KBTotalTimeHuman;

            if (humanMovement.transform.position.x <= this.transform.position.x)
            {
                humanMovement.KnockFromRightHuman = true;
            }

            if (humanMovement.transform.position.x >= this.transform.position.x)
            {
                humanMovement.KnockFromRightHuman = false;
            }
            playerStats.DealDamage(1);
        }
        else if ((collision.gameObject.tag =="Spike")||(collision.gameObject.tag =="Ball"))
        {
            var playerRb = collision.transform.GetComponent<Rigidbody2D>();
            //check the velocity on y axis, once the spike is not down while collision with skeleton, perform transform back to human form
            if (playerRb.velocity.magnitude > velocityThresholdForKilling)
            {
                Destroy(this.gameObject);
            }
            else
            {
                FindObjectOfType<PlayerController>().PerformTransformation(PlayerController.TransformationType.Human);
            }
        }

        else if(collision.gameObject.tag=="Enemy" || collision.gameObject.tag=="Slime")
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
        RaycastHit2D hit = Physics2D.Raycast(transform.position + mRaycastingDistance * raycastDirection, Vector3.down, 1f);

        // if we don't hit something
        if (hit.collider == null)
        {
            //Debug.Log("no colission");
            //Debug.Log(hit.transform.tag.ToString());
            FlipSprite();
        }
    }

    private void FlipSprite()
    {
        bIsGoingRight = !bIsGoingRight;
        _mSpriteRenderer.flipX = bIsGoingRight;
    }



}
