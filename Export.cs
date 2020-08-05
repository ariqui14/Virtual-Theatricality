using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleFileBrowser;
using System.Collections;


//Written by Daniel Thomas
//The filebrowser is a 3rd party asset that is used to get a file location chosen by the user
//exporting a file from the program data location to a user defined location
public class Export : MonoBehaviour
{
    public GameObject itemTemplate;
    public GameObject myContent;
    public Button exportBttn;
    public GameObject blockPanel;
    public Text notificationText;


    private int index = 0;
    private int selectedIndex = -1;
    private List<Button> buttonList = new List<Button>();
    private List<string> pathList = new List<string>();

    private string exportFilePath;

    //Fills the scrollview with all save files in the program folder when the object is enabled
    private void OnEnable()
    {
        exportFilePath = "";
        exportBttn.interactable = false;
        FillList();
        notificationText.gameObject.SetActive(false);
    }

    //Clears the scrollview when the object is disabled
    private void OnDisable()
    {
        exportBttn.interactable = false;
        EmptyList();
    }

    // Update is called once per frame
    //when a item is selected in the scrollview, change the color of the item and enable a button to allow exporting
    void Update()
    {
        if (selectedIndex >= 0 && selectedIndex < buttonList.Count)
        {
            exportBttn.interactable = true;
            buttonList[selectedIndex].GetComponent<Image>().color = Color.grey;
        }
        else
        {
            exportBttn.interactable = false;
        }

        for (int tempIndex = 0; tempIndex < buttonList.Count; tempIndex++)
        {
            if (tempIndex != selectedIndex)
                buttonList[tempIndex].GetComponent<Image>().color = Color.white;
        }
    }

    //function to fill the scrollview of all save files
    private void FillList()
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

    //function to clear scrollview
    private void EmptyList()
    {
        buttonList.Clear();
        pathList.Clear();
        index = 0;
        selectedIndex = -1;

        for(int delete = myContent.transform.childCount - 1; delete >= 0; delete-- )
        {
            Destroy(myContent.transform.GetChild(delete).gameObject);
        }
    }

    //function to be called when the export button is clicked
    //loads the third party file browser with appropraite restrictions
    //Starts couroutine that handles file broswer
    public void Start_OnClick()
    {
        exportFilePath = "";

        // Set filters (optional)
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Virtual Theatricality", ".vt"));

        // Set default filter that is selected when the dialog is shown (optional)
        FileBrowser.SetDefaultFilter(".vt");
        FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe");
        FileBrowser.AddQuickLink("Users", "C:\\Users", null);
        FileBrowser.AddQuickLink("Save Location", Application.persistentDataPath, null);

        StartCoroutine(ShowSaveDialogCoroutine());
    }

    //Export coroutine that lets user interact with file browser and returns the results
    IEnumerator ShowSaveDialogCoroutine()
    {
        blockPanel.SetActive(true);
        // Show a file dialog and wait for a response from user
        yield return FileBrowser.WaitForSaveDialog(true, null, "Select Export Folder", "Select");

        // Dialog is closed, checks for success or failure. Prompts user if successful or not
        if (FileBrowser.Success)
        {
            // If a folder is chosen, return the path
            exportFilePath = FileBrowser.Result;
            notificationText.gameObject.SetActive(true);
            try
            {
                //copy existing save file to new location
                System.IO.File.Copy(Application.persistentDataPath + "/" + buttonList[selectedIndex].GetComponentInChildren<Text>().text + ".vt",
                    exportFilePath + "/" + buttonList[selectedIndex].GetComponentInChildren<Text>().text + ".vt", false);
                notificationText.text = "Export Successful!";
                notificationText.color = new Vector4(0, 100, 0, 255);
            }
            catch
            {
                notificationText.text = "Export Failed!";
                notificationText.color = new Vector4(144, 0, 0, 255);
            }
            blockPanel.SetActive(false);
        }
        else
        {
            notificationText.text = "Export Cancelled!";
            notificationText.color = new Vector4(144, 0, 0, 255);
            blockPanel.SetActive(false);

        }
    }

}
