using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Daniel Thomas
//toggle from double curtain to single curtain on click on the thrust stage
public class ThrustDoubleCurtain : MonoBehaviour
{
    public GameObject doubleCurtain, singleCurtain;

    private void OnMouseDown()
    {
        doubleCurtain.SetActive(false);
        singleCurtain.SetActive(true);
    }
}
