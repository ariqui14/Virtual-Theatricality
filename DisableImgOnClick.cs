using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//Written by Daniel Thomas
//Used for the inital start curtain of the title menu
//disable object on click won't show up again until program has bee reloaded
public class DisableImgOnClick : MonoBehaviour
{
    public Image myImg;

    void Start()
    {
        //checks how long the program has been running for
        if (Time.time > 0)
            myImg.gameObject.SetActive(false);
    }
	// Update is called once per frame
	void Update()
    {
        //left click anywhere, curtain is disable
        if (Input.GetMouseButtonDown(0))
		{
            myImg.gameObject.SetActive(false);
        }

	}
}
