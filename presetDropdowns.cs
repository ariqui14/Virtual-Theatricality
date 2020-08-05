using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

//Build Set UI Preset Stage Dropdown Script.
//Organized and tested by Catherine Stettler.
public class presetDropdowns : MonoBehaviour
{
    public buildFunctions currentBuildFunctions;
    public string folderName;
    public List<GameObject> prefabList;

    private Dropdown myDropdown;

    //Dropdown and fill list functions.
    //Originally written by Daniel Thomas and Ariel Quinan.
    //Variables changed to suit module needs.

    //Awake().
    //Written by Daniel Thomas.
    //Initializes the dropdown object before populating the dropdown list for Stage Presets.
    void Awake()
    {
        myDropdown = gameObject.GetComponent<Dropdown>();
        FillList();

        //for some reason the add actor dropdown won't fill in the label when initialized to 0 but will fill it when the update runs if it starts at a non 0 value
        myDropdown.value = 0;
    }


    //Update().
    //Original code written by Daniel Thomas and Ariel Quinan. Modified by Catherine Stettler.
    //Updates the state of the program at the start of each frame. Associates the addPreset function from
    //the buildFunctions script.
    void Update()
    {
        if (myDropdown.value != 0)
        {
            currentBuildFunctions.addPreset(prefabList[myDropdown.value - 1]);
            myDropdown.value = 0;
        }
    }

    //FillList().
    //Originally written by Daniel Thomas and Ariel Quinan. Modified by Catherine Stettler.
    //Fills the dropdown lists with the appropriate prefab objects.
    private void FillList()
    {
        //Finds prefabs & fills dropdown options
        prefabList = new List<GameObject>();
        GameObject[] tempPrefabs = Resources.LoadAll<GameObject>(folderName);
        myDropdown.ClearOptions();
        if (folderName == "StagePrefabs")
            myDropdown.options.Add(new Dropdown.OptionData("Add Preset"));

        foreach (Object prefab in tempPrefabs)
        {
            GameObject myObject = (GameObject)prefab;
            prefabList.Add(myObject);
            myDropdown.options.Add(new Dropdown.OptionData(myObject.name));
        }
    }
}
