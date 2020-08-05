using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Daniel Thomas
//toggle from single curtain to double curtain on click on the thrust stage
public class ThrustSingleCurtain : MonoBehaviour
{
    public GameObject doubleCurtain;

    private void OnMouseDown()
    {
        gameObject.SetActive(false);
        doubleCurtain.SetActive(true);
    }
}
