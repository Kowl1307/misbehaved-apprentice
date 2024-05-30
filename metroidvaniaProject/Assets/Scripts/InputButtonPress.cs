using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputButtonPress : MonoBehaviour
{
    private static List<string> toCheckAxes = new();
    private static Dictionary<string, float> lastFrameInputs = new();

    private static Coroutine inputUpdaterCoroutine;
    
    private void Start()
    {
        AddAxis("Jump");
        AddAxis("Fire1");
        AddAxis("Fire2");
        AddAxis("Fire3");
    }

    private void OnEnable()
    {
        inputUpdaterCoroutine = StartCoroutine(UpdateFrameInputs());
    }

    private void OnDisable()
    {
        StopCoroutine(inputUpdaterCoroutine);
    }

    private static IEnumerator UpdateFrameInputs()
    {
        while (true)
        {
            foreach (var inputAxis in toCheckAxes)
            {
                lastFrameInputs[inputAxis] = Input.GetAxis(inputAxis);
            }

            yield return new WaitForEndOfFrame();
        }
    }

    public static bool IsPressedThisFrame(string axisName)
    {
        if (!toCheckAxes.Contains(axisName) || !lastFrameInputs.ContainsKey(axisName)) return false;

        return lastFrameInputs[axisName] == 0 && Input.GetAxis(axisName) != 0;
    }

    public static void AddAxis(string axisName)
    {
        try
        {
            //This will fail if the axis doesn't exist
            var input = Input.GetAxis(axisName);
            toCheckAxes.Add(axisName);
        }
        catch (UnityException e)
        {
            Debug.LogWarning(e.StackTrace);
        }
    }


}
