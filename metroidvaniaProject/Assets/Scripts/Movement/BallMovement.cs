using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallMovement : MonoBehaviour, ITransformation
{


    [SerializeField] private float bouncyness;
    [SerializeField] private float boostMultiplier;
    [SerializeField] public int boostCd = 5; // in seconds

    [SerializeField] private Image boostIcon;


    Rigidbody2D _rb;
    Vector2 _lastV;
    bool _cdcheck;
    [SerializeField] bool _pressedBoost;
    public float currentBoostCd;
    [SerializeField] bool _boostact;
    bool _nonote;

    public float maxSpeed = 40f;
    
    //knock back variables
    public float kbForceBall = 10;
    public float kbCounterBall;
    public float kbTotalTimeBall = 0.3f;
    public bool knockFromRightBall;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        currentBoostCd = -boostCd;
        
        boostIcon.fillAmount = 1;
    }

    // Update is called once per frame
    void Update()
    {

        if (kbCounterBall <= 0)
        {
            //pressedBoost = InputButtonPress.IsPressedThisFrame("Jump");
            if (!_pressedBoost)
                _pressedBoost = InputButtonPress.IsPressedThisFrame("Jump");
            //_pressedBoost = Input.GetAxis("Jump") > .1f;
        }
        //knockback
        else
        {
            if (knockFromRightBall == true)
            {
                _rb.velocity = new Vector2(-kbForceBall, kbForceBall);
            }

            if (knockFromRightBall == false)
            {
                _rb.velocity = new Vector2(kbForceBall, kbForceBall);
            }

            kbCounterBall -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        _lastV = _rb.velocity;
        Boost();
        _rb.velocity = Vector2.ClampMagnitude(_rb.velocity, maxSpeed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var speed = _lastV.magnitude;
        var dir = Vector2.Reflect(_lastV.normalized, collision.GetContact(0).normal);
        if (_boostact)
        {
            _rb.velocity = dir * Mathf.Max(speed * (bouncyness * boostMultiplier), 0f);
            _boostact = false;
            currentBoostCd = Time.fixedTime;
            boostIcon.fillAmount = 0;
        }
        else
        {
            _rb.velocity = dir * Mathf.Max(speed * bouncyness, 0f);
        }
    }

    private void Boost()
    {
        _cdcheck = (currentBoostCd + boostCd) < Time.fixedTime;
        if (_pressedBoost && _cdcheck)
        {
            _boostact = true;
            _pressedBoost = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "MaxSpeed") {
            maxSpeed = 47f;
            boostMultiplier = 1.4f;
        }
        if (collision.tag == "NormalSpeed")
        {
            maxSpeed = 30f;
            boostMultiplier = 1.3f;
        }
    }

 
    public void InitTransformation()
    {

    }

    public void OnLeaveTransformation()
    {

    }
}

