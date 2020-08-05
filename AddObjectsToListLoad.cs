using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//Created by Daniel Thomas
//This script is meant to populate the load scrollview with all the appropriate files
public class AddObjectsToListLoad : MonoBehaviour
{
    //public variables assigned in inspector
    public GameObject itemTemplate;
    public GameObject myContent;
    public Button deleteBttn;
    public Button loadBttn;

    private int index = 0;
    private int selectedIndex = -1;
    private List<Button> buttonList = new List<Button>();
    private List<string> pathList = new List<string>();

    //When object is enabled, list is dynamically reloaded to accomodate any changes
    private void OnEnable()
    {
        deleteBttn.interactable = false;
        loadBttn.interactable = false;
        FillList();
    }

    //When object is disabled, list is cleared in order to avoid duplicates on reload
    private void OnDisable()
    {
        deleteBttn.interactable = false;
        loadBttn.interactable = false;
        EmptyList();
    }

    // Update is called once per frame
    //every frame, the game checks to see if an option is selected and changes buttons to active/inactive
    void Update()
    {
        if (selectedIndex >= 0 && selectedIndex < buttonList.Count)
        {
            deleteBttn.interactable = true;
            loadBttn.interactable = true;
            buttonList[selectedIndex].GetComponent<Image>().color = Color.grey;
        }
        else
        {
            deleteBttn.interactable = false;
            loadBttn.interactable = false;
        }

        for (int tempIndex = 0; tempIndex < buttonList.Count; tempIndex++)
        {
            if (tempIndex != selectedIndex)
                buttonList[tempIndex].GetComponent<Image>().color = Color.white;
        }
    }

    //Function that fills the scrollview wth all the save files
    public void FillList()
    {
        string[] files = System.IO.Directory.GetFiles(Application.persistentDataPath, "*.vt*");
        for (index = 0; index < files.Length; index++)
        {
            var copy = Instantiate(itemTemplate);
            copy.transform.SetParent(myContent.transform);
            copy.GetComponentInChildren<Text>().text = System.IO.Path.GetFileNameWithoutExtension(files[index]);



            copy.GetComponent<Button>().onClick.AddListener
                (
                    () =>
                    {
                        selectedIndex = buttonList.FindIndex(myInput => myInput.GetComponent<Button>() == copy.GetComponent<Button>());
                    }
                );

            buttonList.Add(copy.GetComponent<Button>());
            pathList.Add((string)System.IO.Path.GetFileName(files[index]));
        }
    }

    //function that clears the scrollview of all items
    private void EmptyList()
    {
        buttonList.Clear();
        pathList.Clear();
        index = 0;
        selectedIndex = -1;

        for (int delete = myContent.transform.childCount - 1; delete >= 0; delete--)
        {
            Destroy(myContent.transform.GetChild(delete).gameObject);
        }
    }

    //Function for when the delete button is pressed, deletes the selected file from the computer
    public void DeleteButton_Click()
    {
        System.IO.File.Delete(Application.persistentDataPath + "/" + pathList[selectedIndex]);
        Destroy(myContent.transform.GetChild(selectedIndex).gameObject);
        buttonList.RemoveAt(selectedIndex);
        pathList.RemoveAt(selectedIndex);
        selectedIndex = -1;
    }

    //Function for when the load button is pressed, loads the selected file and changes the scene based on the data
    public void LoadSceneButton_Click()
    {
        SessionData.myFileName = buttonList[selectedIndex].GetComponentInChildren<Text>().text;
        Save data = SaveSystem.loadData();
        SessionData.selectedStageType = data.stageType;
        SceneManager.LoadScene(SessionData.selectedStageType);
    }

}
