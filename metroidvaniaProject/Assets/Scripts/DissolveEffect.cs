using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveEffect : MonoBehaviour
{
    [SerializeField] private Material material;
    private float Dissolvefloat;  // Used for controlling how well we can see the objects
    private float dissolveSpeed = 4f;
    private bool isDissolving;

    

    // Update is called once per frame
    private void Update()
    {
        if (isDissolving)
        {

            Dissolvefloat = Mathf.Clamp01(Dissolvefloat -  dissolveSpeed * Time.deltaTime);
            material.SetFloat("_Dissolvefloat", Dissolvefloat);
        }
        else
        {

            Dissolvefloat = Mathf.Clamp01(Dissolvefloat + dissolveSpeed * Time.deltaTime);
            material.SetFloat("_Dissolvefloat", Dissolvefloat);
        }
        
        
    }

    public void Disappear(float dissolveSpeed)
    {
        

        this.dissolveSpeed = dissolveSpeed;

        material.SetFloat("_Dissolvefloat", 1f);
        Dissolvefloat = 1f;
        isDissolving = true;

    }// Dissolve the sprite
    public void Reappear(float dissolveSpeed)
    {
        
        this.dissolveSpeed = dissolveSpeed;

        material.SetFloat("_Dissolvefloat", 0f);
        Dissolvefloat = 0f;
        isDissolving = false;
    }


}
