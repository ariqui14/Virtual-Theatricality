//Daniel
//This is the pause script to enable/disable the pause menu in the game scene
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    public GameObject myPause;
    // Start is called before the first frame update
    void Start()
    {
        myPause.gameObject.SetActive(false);
    }

    // Update is called once per frame
    //It checks if the "Escape" key is pressed, and if so, executes the function.
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (myPause.activeSelf)
            {
                myPause.gameObject.SetActive(false);
            }
            else
            {
                myPause.gameObject.SetActive(true);
            }
        }
    }
}
