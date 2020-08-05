using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class customMeasure : MonoBehaviour
{
    public Button addButton;
    public GameObject newObject;

    public InputField inputX;
    public InputField inputY;

    private int xVal, yVal;
    private string standrd = "25";

    // Start is called before the first frame update
    void Start()
    {
        addButton.onClick.AddListener(onCustomClick);
        inputX.text = standrd;
        inputY.text = standrd;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void setX(InputField userInput)
    {
        if(userInput.text != standrd)
            xVal = int.Parse(userInput.text);
        else
            xVal = int.Parse(standrd);
    }

    private void setY(InputField userInput)
    {
        if (userInput.text != standrd)
            yVal = int.Parse(userInput.text);
        else
            yVal = int.Parse(standrd);
    }

    public void onCustomClick()
    {
        setX(inputX);
        setY(inputY);

        newObject.transform.localScale = new Vector3(xVal, 1, yVal);

        Instantiate(newObject, new Vector3(0, 0, 0), Quaternion.identity);
    }
}
