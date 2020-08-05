//Daniel
//The filebrowser is a 3rd party asset that is used to get a file location chosen by the user
//importing a file from a user defined location to a program data location.
using UnityEngine;
using UnityEngine.UI;
using SimpleFileBrowser;
using System.Collections;

public class Import : MonoBehaviour
{
    public Button importBttn;
    public Text notificationText;
    public GameObject outerPanel;



    private string importFilePath;

    // Start is called before the first frame update
    void Start()
    {
        importFilePath = "";
        importBttn.interactable = true;
        notificationText.gameObject.SetActive(false);
    }

    //This function is called when the "Import" button is clicked
    public void Start_OnClick()
    {
        importFilePath = "";

        // Set filters (optional)
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Virtual Theatricality", ".vt"));

        // Set default filter that is selected when the dialog is shown (optional)
        FileBrowser.SetDefaultFilter(".vt");

        FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe");

        FileBrowser.AddQuickLink("Users", "C:\\Users", null);
        FileBrowser.AddQuickLink("Save Location", Application.persistentDataPath, null);

        StartCoroutine(ShowSaveDialogCoroutine());
    }

    //This Coroutine handles the filebrowser for importing and returns the location the file that the user chose
    IEnumerator ShowSaveDialogCoroutine()
    {
        // Show a load file dialog and wait for a response from user
        yield return FileBrowser.WaitForSaveDialog(false, null, "Select Theatre to Import", "Import");

        // Dialog is closed

        notificationText.gameObject.SetActive(true);
        if (FileBrowser.Success)
        {
            // If a folder is chosen, return the path
            importFilePath = FileBrowser.Result;
            string[] fileName;
            if (Application.platform != RuntimePlatform.WindowsPlayer && Application.platform != RuntimePlatform.WindowsEditor)
                fileName = importFilePath.Split('/');
            else
                fileName = importFilePath.Split('\\');

            //copy existing save file to new location
            try
            {

                System.IO.File.Copy(importFilePath, Application.persistentDataPath + "/" + fileName[fileName.Length - 1]);
                notificationText.text = "Import Successful!";
                notificationText.color = new Vector4(0, 100, 0, 255);
            }
            catch
            {
                notificationText.text = "Import Failed!";
                notificationText.color = new Vector4(144, 0, 0, 255);
            }

        }
        else
        {
            notificationText.text = "Import Cancelled!";
            notificationText.color = new Vector4(144,0,0,255);
        }
    }
}
