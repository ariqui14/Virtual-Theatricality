using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Block Scene UI Add Props and Add Actors Dropdown Script.
//Organized and tested by Daniel Thomas.
public class MenuDropdown : MonoBehaviour
{
    public MenuFunctions currentMenuFunctions;
    public string folderName;
    public List<GameObject> prefabList;


    private Dropdown myDropdown;

    //Dropdown and fill list functions.
    //Written by Daniel Thomas and Ariel Quinan.
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
    //Written by Daniel Thomas and Ariel Quinan.
    //Updates the state of the program at the start of each frame. Associates the AddObjectFunction function from
    //the MenuFunctions script.
    void Update()
    {
        if (myDropdown.value != 0)
        {
            currentMenuFunctions.AddObjectFunction(prefabList[myDropdown.value-1]);
            myDropdown.value = 0;
        }
    }


    //FillList().
    //Written by Daniel Thomas and Ariel Quinan.
    //Fills the dropdown lists with the appropriate prefab objects.
    private void FillList()
    {
        //find prefabs
        prefabList = new List<GameObject>();
        GameObject[] tempPrefabs = Resources.LoadAll<GameObject>(folderName);
        myDropdown.ClearOptions();
        if(folderName == "PropPrefabs")
            myDropdown.options.Add(new Dropdown.OptionData("Add Props"));
        else if(folderName == "ActorPrefabs")
            myDropdown.options.Add(new Dropdown.OptionData("Add Actor"));

        foreach (Object prefab in tempPrefabs)
        {
            GameObject myObject = (GameObject)prefab;
            prefabList.Add(myObject);
            myDropdown.options.Add(new Dropdown.OptionData(myObject.name));
        }
    }
}