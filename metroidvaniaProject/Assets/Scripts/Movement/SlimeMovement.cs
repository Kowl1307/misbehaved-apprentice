using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class SlimeMovement : MonoBehaviour, ITransformation
{
    private Rigidbody2D rb2d;
    [SerializeField] private Transform transformofSlime;
    [SerializeField] private float SlimeSpeed = 5f;
    [SerializeField] private float slowFallSpeed = 50f;
    [SerializeField] private LayerMask movlableGround;
    
    private float Input_x;
    private float Input_y;
    private bool insidePipe = false;

    [SerializeField] public float absorbCD = 10f;
    [SerializeField] private float absorbDur = 5f;
    [SerializeField] private Image absorbIcon;

    [SerializeField] private GameObject blueFlame;

    public bool cdcheck;
    public bool absorbCheck;
    bool note=false;
    bool note2=false;
    float currentAbsorbCD;
    private bool abilityPressed;

    private BoxCollider2D coll;
    private PlayerStats playerStats;


    private void Awake()
    {
        playerStats = this.gameObject.GetComponentInParent<PlayerStats>();
        rb2d = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
    }

    void Start()
    {
        currentAbsorbCD = -absorbCD;
    }

    // Update is called once per frame
    void Update()
    {
        Input_x = Input.GetAxis("Horizontal");
        Input_y = Input.GetAxisRaw("Vertical");
        HandleWalking();
        abilityPressed = Input.GetAxis("Jump") > .1f;

        if(Input.GetAxis("Jump") > 0)
        {
            var overlaps = Physics2D.OverlapBoxAll(transform.position, Vector2.one, 0).ToList();
            if (overlaps.Count <= 0) return;
            var firstPipe = overlaps.Find(collider => collider.GetComponent<Pipegates>() != null);
            if(firstPipe != null)
                firstPipe.GetComponent<Pipegates>().ActivatePipe();
        }
       
    }

    private void FixedUpdate()
    {
        cdcheck = (currentAbsorbCD + absorbCD) < Time.fixedTime;
        absorbCheck = (currentAbsorbCD + absorbDur) < Time.fixedTime;
        if (((currentAbsorbCD + absorbDur) < Time.fixedTime) && note2)
        {
            Debug.Log("Absorb Ends");
            note2 = false;
        }

        if (cdcheck && note)
        {
            Debug.Log("Absorb Ready"); note = false;
        }

        if (abilityPressed && cdcheck)
        {
            StartAbsorb();
        };
    }

    private void HandleWalking()
    {
        if (!IsGrounded()) return;
        
        var targetSpeed = Input_x * SlimeSpeed;
        var speedDif = targetSpeed - rb2d.velocity.x;
        var accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? 1 : 5;
        var movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, 1.3f) * Mathf.Sign(speedDif);
        rb2d.AddForce(new Vector2(movement, 0));

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Walls" || collision.gameObject.name =="MovingWall")
        {
            rb2d.drag = slowFallSpeed;
        }
    }

     private void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log("Slime Collision: "+collision.gameObject.name.ToString());
        if (collision.gameObject.name == "Walls" || collision.gameObject.name == "MovingWall")
        {
            rb2d.drag = 0;
        }
    }

    private void StartAbsorb() {
        //Debug.Log("Absorb aktive");
        currentAbsorbCD = Time.fixedTime;

        absorbIcon.fillAmount = 0;
        playerStats.absorbStart = currentAbsorbCD;
        playerStats.absorbDur = absorbDur;
        note = true;
        note2 = true;
        playerStats.inv = true;
    }

    public void AbsorbFire()
    {
        IEnumerator RemoveFireAfterSeconds(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            blueFlame.SetActive(false);
        }
        
        blueFlame.SetActive(true);
        playerStats.StartCoroutine(RemoveFireAfterSeconds(FireDamage.fireAbsorbDuration));
    }

    private bool IsGrounded()
    {
        var contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(LayerMask.GetMask("Ground"));
        var list = new List<Collider2D>();
        return coll.OverlapCollider(contactFilter, list) > 0;
        //return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, LayerMask.GetMask("Ground"));
    }
    public void InitTransformation()
    {
        playerStats.inv = true;
    }

    public void OnLeaveTransformation()
    {
        // dissolveEffect.Disappear(3f);

        playerStats.inv = false;
    }
}
