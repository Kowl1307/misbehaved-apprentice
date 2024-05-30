using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityIconCD : MonoBehaviour
{

    [SerializeField] private Image boostIcon;
    [SerializeField] private BallMovement ballMove;


    [SerializeField] private Image absorbIcon;
    [SerializeField] private SlimeMovement slimeMove;


    // Start is called before the first frame update
    void Start()
    {
        boostIcon.fillAmount = 0;
        absorbIcon.fillAmount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        boostIcon.fillAmount += 1f / ballMove.boostCd * Time.deltaTime;
        absorbIcon.fillAmount += 1f / slimeMove.absorbCD * Time.deltaTime;
    }
}
