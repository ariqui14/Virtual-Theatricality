using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Build Set UI Prop and Set Structure Dropdown Script.
//Organized and tested by Catherine Stettler.
public class buildDropdowns : MonoBehaviour
{
    public buildFunctions currentBuildFunctions;
    public string folderName;
    public List<GameObject> prefabList;

    public Dropdown myDropdown;

    //Dropdown and fill list functions.
    //Originally written by Daniel Thomas and Ariel Quinan.
    //Variables changed to suit module needs.

    //Awake().
    //Written by Daniel Thomas.
    //Initializes the dropdown object before populating the dropdown list for Props and Set Structures.
    void Awake()
    {
        myDropdown = gameObject.GetComponent<Dropdown>();
        FillList();

        myDropdown.value = 0;
    }

    //Update().
    //Original code written by Daniel Thomas and Ariel Quinan. Modified by Catherine Stettler.
    //Updates the state of the program at the start of each frame. Associates the AddItems function from
    //the buildFunctions script.
    void Update()
    {
        if (myDropdown.value != 0)
        {
            currentBuildFunctions.AddItems(prefabList[myDropdown.value - 1]);
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
        if (folderName == "PropPrefabsBuildUI")
            myDropdown.options.Add(new Dropdown.OptionData("Add Prop"));
        else if(folderName == "SetPrefabs")
            myDropdown.options.Add(new Dropdown.OptionData("Add Structure"));

        foreach (Object prefab in tempPrefabs)
        {
            GameObject myObject = (GameObject)prefab;
            prefabList.Add(myObject);
            myDropdown.options.Add(new Dropdown.OptionData(myObject.name));
        }
    }
}
