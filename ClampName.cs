using UnityEngine;

//Written by Daniel Thomas
//used to orientate a nametag of an actor to face the screen
public class ClampName : MonoBehaviour
{
    GameObject myCanvas;
    // Start is called before the first frame update
    void Start()
    {
        //canvas of actor name object
        myCanvas = (GameObject)this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        //face the active camera
        myCanvas.transform.eulerAngles = Camera.main.gameObject.transform.eulerAngles;
    }
}
