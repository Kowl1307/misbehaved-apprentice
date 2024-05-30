using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public enum TransformationType
    {
        Human,
        Ball,
        Spike,
        Slime
    }

    public TransformationType CurrentTransformation { get; private set; }

    private CameraFollow cameraFollowScript;

    //Change this from MonoBehaviour to something else
    [SerializeField] private MonoBehaviour humanMovementScript;
    [SerializeField] private MonoBehaviour ballMovementScript;
    [SerializeField] private MonoBehaviour spikeMovementScript;
    [SerializeField] private MonoBehaviour slimeMovementScript;

    public GameObject HumanObject => humanMovementScript.gameObject;
    public GameObject BallObject => ballMovementScript.gameObject;
    public GameObject SpikeObject => spikeMovementScript.gameObject;
    public GameObject SlimeObject => slimeMovementScript.gameObject;

    [SerializeField] private TransformationType bTransformation;
    [SerializeField] private TransformationType xTransformation;
    [SerializeField] private TransformationType yTransformation;

    public GameObject CurrentPlayerObject => GetCurrentMovementScript().gameObject;

    [SerializeField] private bool showLogs;
    [SerializeField] private DissolveEffect dissolveEffect;

    private void Awake()
    {
        cameraFollowScript = FindObjectOfType<CameraFollow>();
    }

    private void Start()
    {

        //Deactivate all movement scripts except the human form
        DisableAllMovementScripts();
        DisableAllPlayerObjects();

        humanMovementScript.enabled = true;
        humanMovementScript.gameObject.SetActive(true);

        //PerformTransformation(TransformationType.Human);
        
    }

    public void Update()
    {
        var bPressed = InputButtonPress.IsPressedThisFrame("Fire1");
        var xPressed = InputButtonPress.IsPressedThisFrame("Fire2");
        var yPressed = InputButtonPress.IsPressedThisFrame("Fire3");
        
        if (showLogs)
        {
            Debug.Log("b pressed: " + bPressed);
            Debug.Log("x pressed: " + xPressed);
            Debug.Log("y pressed: " + yPressed);
        }

        //If no transformation was pressed, we don't need to do anything
        if (!bPressed && !yPressed && !xPressed) return;



        var targetTransformation = bPressed ? bTransformation : (yPressed ? yTransformation : xTransformation);
        //If the desired transformation is the current one, transform back to Human
        if (targetTransformation == CurrentTransformation) targetTransformation = TransformationType.Human;

        PerformTransformation(targetTransformation);




    }


    public void PerformTransformation(TransformationType targetTransformation)
    {

        //Disable the current player object
        GetCurrentMovementScript().GetComponent<ITransformation>().OnLeaveTransformation();

        var oldTransformationObject = CurrentPlayerObject;
        var oldPosition = CurrentPlayerObject.transform.position;

        //Play animation
        dissolveEffect.Reappear(3f);

        //Enable the new transformation and activate its gameObject
        EnableMovementScript(targetTransformation);

        CurrentPlayerObject.SetActive(true);

        //Conserve velocity between transformations
        GetCurrentMovementScript().GetComponent<Rigidbody2D>().velocity = oldTransformationObject.GetComponent<Rigidbody2D>().velocity;
        GetCurrentMovementScript().GetComponent<ITransformation>().InitTransformation();
        Debug.Log("Old Velocity: " + oldTransformationObject.GetComponent<Rigidbody2D>().velocity);
        Debug.Log("New Velocity: " + CurrentPlayerObject.GetComponent<Rigidbody2D>().velocity);

        CurrentPlayerObject.transform.position = oldPosition;

        oldTransformationObject.SetActive(false);
        cameraFollowScript.target = CurrentPlayerObject.transform;
    }



        /// <summary>
        /// Disables all movement scripts except the one for the given transformation
        /// </summary>
        /// <param name="targetTransformationType"></param>
        public void EnableMovementScript(TransformationType targetTransformationType)
        {
            DisableAllMovementScripts();
            switch (targetTransformationType)
            {
                case TransformationType.Human:
                    humanMovementScript.enabled = true;
                    break;
                case TransformationType.Ball:
                    ballMovementScript.enabled = true;
                    break;
                case TransformationType.Spike:
                    spikeMovementScript.enabled = true;
                    break;
                case TransformationType.Slime:
                    slimeMovementScript.enabled = true;
                    break;
            }

            CurrentTransformation = targetTransformationType;
        }

        private void DisableAllMovementScripts()
        {
            humanMovementScript.enabled = false;
            ballMovementScript.enabled = false;
            spikeMovementScript.enabled = false;
            slimeMovementScript.enabled = false;
        }

        private void DisableAllPlayerObjects()
        {
            humanMovementScript.gameObject.SetActive(false);
            ballMovementScript.gameObject.SetActive(false);
            spikeMovementScript.gameObject.SetActive(false);
            slimeMovementScript.gameObject.SetActive(false);
        }

        private MonoBehaviour GetCurrentMovementScript()
        {
            switch (CurrentTransformation)
            {
                case TransformationType.Human:
                    return humanMovementScript;
                case TransformationType.Ball:
                    return ballMovementScript;
                case TransformationType.Spike:
                    return spikeMovementScript;
                case TransformationType.Slime:
                    return slimeMovementScript;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

       

    } 
