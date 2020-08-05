//Daniel
//This class creates a new stage UI from the intro menu when creating a new theatre. 
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ShowCreateTheatre : MonoBehaviour
{
    public Dropdown myDropdown;
    public Button myButton;
    public  Text customTheatreName;
    private string sceneName;
  
    //The Start function disables the "New Stage" button on load
    void Start()
    {
        myButton.interactable = false;
    }

    // Update is called once per frame
    //This determines if an option from the dropdown has been chosen and enables the "New Stage" if it has been chosen.
    void Update()
    {
        if (myDropdown.captionText.text == "Proscenium")
        {
            sceneName = "Proscenium";
            myButton.interactable = true;
        }
        else if (myDropdown.captionText.text == "Thrust")
        {
            sceneName = "Thrust";
            myButton.interactable = true;
        }
        else if (myDropdown.captionText.text == "Theatre-in-the-Round")
        {
            sceneName = "InTheRound";
            myButton.interactable = true;
        }
        else if (myDropdown.captionText.text == "Custom")
        {
            sceneName = "Custom";
            myButton.interactable = true;
        }
        else
        {
            myButton.interactable = false;
        }
    }

    //This function changes to a new scene, the scene name is given in inspector
    //Changes global data of new file
    public void NewLevel()
    {
        SessionData.selectedStageType = sceneName;
        if (customTheatreName.text != "" && customTheatreName.text != null)
            SessionData.myFileName = customTheatreName.text;
        else
            SessionData.myFileName = null;

        Debug.Log("File Name: " + SessionData.myFileName);
        SceneManager.LoadScene(sceneName);
    }

}

