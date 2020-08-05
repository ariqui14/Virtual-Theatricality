using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


//All created by Daniel Thomas
public class TitleMenu : MonoBehaviour
{

    public Font uiFont;

    //Changes all font to what is assigned in the inspector when script loads
    private void Awake()
    {
        var textComponents = Resources.FindObjectsOfTypeAll<Text>();
        foreach (var component in textComponents)
            component.font = uiFont;
    }

    void Start()
    {
        SessionData.myFileName = null;//reset new theatre name in memory
        Screen.SetResolution(1920, 1080, true);//set screen size to 1920x1080
    }
    //changes to new scene, scene name given in inspector
    public void ChangeLevel(string lvlName)
    {
        SceneManager.LoadScene(lvlName);
    }

    //reloads current scene
    public void RestartLvl()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //Sets a game object to active
    public void EnableObject(GameObject myObject)
    {
        myObject.SetActive(true);
    }

    //Sets a game object to inactive
    public void DisableObject(GameObject myObject)
    {
        myObject.SetActive(false);
    }

    //quits game
    public void QuitGame()
    {
        Application.Quit();
    }

    //clears a text component to an empty string
    public void ResetText_OnClick(Text myText)
    {
        myText.text = "";
    }
}
