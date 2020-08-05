using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

//Build Set UI Module Script
//Organized and tested by Catherine Stettler.
public class buildFunctions : MonoBehaviour
{
    public Button clearSet;
    public Button deleteStage;
    public Button remObject;

    public buildDropdowns props;
    public buildDropdowns structures;
    public presetDropdowns preset;
    public customDropdown custom;

    private float moveSpeed = 0.15f;

    public GameObject selectionCirclePrefab;
    private GameObject selectedObject;
    public MenuFunctions menuFunctions; //Assigned in inspector for save purposes

    public List<GameObject> setList = new List<GameObject>();

    private int stageIndex = 0;
    private bool unitTest = true;

    // Start is called before the first frame update
    void Start()
    {
        //Unit testing. Will run only if unitTest = true.
        unitTestProps();
        unitTestCustom();
        unitTestPresets();
        unitTestStructures();
        if (unitTestInspector() != false)
            print("All inspector elements are assigned. No errors found.");
        else
            print("Inspector element unassigned. Functionality may be impaired.");
    }

    //Update.
    //Original code written by Daniel Thomas and Ariel Quinan. Modified by Catherine Stettler.
    //Updates the state of the program at the start of each frame. Also handles movement and manipulation of the
    //static props and set pieces.
    void Update()
    {

        if (EventSystem.current.IsPointerOverGameObject())//if mouse is over a UI menu, return
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        //Select an object and spawn the selection circle. Originally written by Daniel and Ariel.
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform != null && hit.transform.gameObject.CompareTag("BuildProps"))
                {
                    //turn off selection circle of previously selected object
                    if (selectedObject != null && selectedObject.transform.gameObject.CompareTag("BuildProps"))
                        selectedObject.transform.Find("SelectionCylinder(Clone)").gameObject.SetActive(false);

                    selectedObject = setList.Find(myInput => myInput == hit.transform.gameObject);
                    if(selectedObject.transform.gameObject.CompareTag("BuildProps"))
                        selectedObject.transform.Find("SelectionCylinder(Clone)").gameObject.SetActive(true);
                }
                else if(hit.transform != null && hit.transform.gameObject.CompareTag("Set"))
                {
                    selectedObject = setList.Find(myInput => myInput == hit.transform.gameObject);
                }
            }
        }
        //Deselects an object on right click.
        if (Input.GetMouseButtonDown(1))
        {
            deselectObject();
        }
        /*//rotates an object
        if (GameObject.Find("Dropdown List") == null) //detects if any dropdown is open
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0 && selectedObject != null)
            {
                selectedObject.transform.Rotate(Vector3.down * 10f, Space.World);
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0 && selectedObject != null)
            {
                selectedObject.transform.Rotate(Vector3.up * 10f, Space.World);
            }
        }*/
        //Daniel
        //moves and moves an object
        if (selectedObject != null)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                selectedObject.transform.position += Vector3.right * Time.deltaTime * 2;
            }
            else if (Input.GetKey(KeyCode.UpArrow))
            {
                selectedObject.transform.position += Vector3.back * Time.deltaTime * 2;
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                selectedObject.transform.position += Vector3.forward * Time.deltaTime * 2;
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                selectedObject.transform.position += Vector3.left * Time.deltaTime * 2;
            }

            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                selectedObject.transform.Rotate(Vector3.down * 10f, Space.World);
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                selectedObject.transform.Rotate(Vector3.up * 10f, Space.World);
            }
        }
    }

    //setStageIndex().
    //Written by Catherine Stettler.
    //Utility function to get index/indices of the stage in order to clear a set of props and structures, but not the stage floor
    //or stage preset.
    private void setStageIndex()
    {
        int index = 0;

        while(index < setList.Count)
        {
            if (setList[index].CompareTag("Ground") == true)
            {
                stageIndex++;
            }
            index++;
        }
    }
    
    //addPreset(GameObject preset).
    //Written by Catherine Stettler.
    //Adds the selected stage preset to the current scene, spawning the preset at the origin.
    public void addPreset(GameObject preset)
    {
        GameObject stage = Instantiate(preset, new Vector3(0, 0, 0), Quaternion.identity);
        setList.Add(stage);
    }

    //addCustom(GameObject newStage).
    //Written by Catherine Stettler.
    //Adds a custom/non-standard set floor to the current scene, spawning at the origin.
    public void addCustom(GameObject newStage)
    {
        GameObject custom = Instantiate(newStage, new Vector3(0, 0, 0), Quaternion.identity);
        setList.Add(custom);
    }

    //AddItems(GameObject newItem).
    //Original code written by Daniel Thomas and Ariel Quinan. Modified by Catherine Stettler.
    //Calls a coroutine to add a prop to the stage at the point where the user clicks.
    public void AddItems(GameObject newItem)
    {
       // Debug.Log("Static item added.");
        StartCoroutine(WaitForAnswerAddItem(newItem));
    }

    //deleteSet().
    //Written by Catherine Stettler.
    //Iteratively goes through the setList object and removes set items and stationary props from the
    //stage without destroying the stage floor or stage preset.
    public void deleteSet()
    {
        int deleteIndex = 0;
        setStageIndex();

        while(setList.Count > deleteIndex)
        {
            GameObject temp = setList[deleteIndex];

            if (temp.CompareTag("Ground") == true)
            {
                deleteIndex++;
            }
            else if((temp.CompareTag("Set") == true) || (temp.CompareTag("BuildProps") == true))
            {
                Destroy(temp);
                setList.RemoveAt(deleteIndex);
            }
        }
    }

    //deleteStg().
    //Written by Catherine Stettler.
    //Delete stage floor(s) or a stage preset from the scene without destroying the props and
    //set pieces.
    public void deleteStg()
    {
        int deleteIndex = 0;

        while (setList.Count > deleteIndex)
        {
            GameObject temp = setList[deleteIndex];
            if (temp.CompareTag("Ground") == true)
            {
                Destroy(temp);
                setList.RemoveAt(deleteIndex);
            }
            else
            {
                deleteIndex++;
            }
        }
    }

    //objRemove().
    //Written by Daniel Thomas.
    //Calls a coroutine to destroy an object when clicked on by the user.
    public void objRemove()
    {
        StartCoroutine(WaitForAnswerDestroy());
    }

    //deselectObject().
    //Written by Catherine Stettler.
    //Called from the Update function when the user right clicks on the stage or anywhere on the scene
    //away from the currently selected object.
    private void deselectObject()
    {
        if (selectedObject.transform.gameObject.CompareTag("BuildProps"))
            selectedObject.transform.Find("SelectionCylinder(Clone)").gameObject.SetActive(false);
        selectedObject = null;
    }

    //moveObject(GameObject selectedObject).
    //Written by Catherine Stettler.
    //Original move function for props and set pieces - modified for use in the Update() function.
    /*private void moveObject(GameObject selectedObject)
    {
        if(selectedObject != null)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Vector3 pos = selectedObject.transform.localPosition;
                pos.z += moveSpeed * Time.deltaTime;
                selectedObject.transform.position = pos;
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                Vector3 pos = selectedObject.transform.position;
                pos.z++;
                selectedObject.transform.position = pos;
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Vector3 pos = selectedObject.transform.position;
                pos.x++;
                selectedObject.transform.position = pos;
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                Vector3 pos = selectedObject.transform.position;
                pos.x--;
                selectedObject.transform.position = pos;
            }
        }
    }*/

    //Coroutines
    //Originally written by Daniel Thomas.
    //Variable changes to reflect needs for build UI

    //WaitForAnswerAddItem(GameObject newObject).
    //Originally written by Daniel Thomas. Modified by Catherine Stettler.
    //Adds an object at a point where the user clicks on the stage.
    IEnumerator WaitForAnswerAddItem(GameObject newObject)
    {
        for (; ; )
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                //spawns object on click
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform != null && hit.transform.gameObject.CompareTag("Ground"))
                    {
                        GameObject temp = Instantiate(newObject, hit.point + newObject.transform.position, newObject.transform.rotation);
                        GameObject newSelectionCircle = Instantiate(selectionCirclePrefab);
                        newSelectionCircle.transform.SetParent(temp.transform);
                        newSelectionCircle.SetActive(false);
                        menuFunctions.setPieceDropdownIndexes.Add(props.myDropdown.value); 
                        setList.Add(temp);
                        
                        menuFunctions.setList.Add(temp); 
                    }
                }
                break;
            }
            yield return null;
        }

    }

    //WaitForAnswerDestroy().
    //Originally written by Daniel Thomas. Modified by Catherine Stettler.
    //Removes an object when it is clicked on by the user.
    IEnumerator WaitForAnswerDestroy()
    {
        for (; ; )
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if(Physics.Raycast(ray, out hit))
                {
                    if(hit.transform != null && (hit.transform.gameObject.CompareTag("BuildProps") || hit.transform.gameObject.CompareTag("Set")))
                    {
                        int tempIndex = setList.FindIndex(myObject => myObject == hit.transform.gameObject);

                        setList.RemoveAt(tempIndex);
                        Destroy(hit.transform.gameObject);
                    }
                }
                break;
            }
            yield return null;
        }

    }


    //Unit testing.
    //unitTestProps().
    //Written by Catherine Stettler.
    //This function checks to see if a prefab exists in the resource folder and adds it to the setList. Mostly for debugging,
    //the function will print to the console if the object exists, that it was added to the list, the object's name, and then the
    //setList containing the existing prefabs.
    private bool unitTestProps()
    {
        GameObject[] tempList = Resources.LoadAll<GameObject>("PropPrefabsBuildUI");

        if (unitTest != false)
        {
            foreach (Object temporary in tempList)
            {
                GameObject temp = (GameObject)temporary;
                if (temp.GetType() != null)
                {
                    print("Object exists. Adding to setList.");
                    setList.Add(temp);
                    print(temp.name + " added to setList.");
                }
                else
                    print("Object " + temp.name + " does not exist. Cannot add to setList.");
            }

            for(int index = 0; index < setList.Count; index++)
            {
                print(setList[index].name);
            }
            print("Test complete. Clearing setList.");
            setList.Clear();
        }
        return true;
    }
    //unitTestStructures().
    //Written by Catherine Stettler.
    //This function checks to see if a prefab exists in the resource folder and adds it to the setList. Mostly for debugging,
    //the function will print to the console if the object exists, that it was added to the list, the object's name, and then the
    //setList containing the existing prefabs.
    private bool unitTestStructures()
    {
        GameObject[] tempList = Resources.LoadAll<GameObject>("SetPrefabs");

        if (unitTest != false)
        {
            foreach (Object temporary in tempList)
            {
                GameObject temp = (GameObject)temporary;
                if (temp.GetType() != null)
                {
                    print("Object exists. Adding to setList.");
                    setList.Add(temp);
                    print(temp.name + " added to setList.");
                }
                else
                    print("Object " + temp.name + " does not exist. Cannot add to setList.");
            }

            for (int index = 0; index < setList.Count; index++)
            {
                print(setList[index].name);
            }
            print("Test complete. Clearing setList.");
            setList.Clear();
        }
        return true;
    }
    //unitTestPresets().
    //Written by Catherine Stettler.
    //This function checks to see if a prefab exists in the resource folder and adds it to the setList. Mostly for debugging,
    //the function will print to the console if the object exists, that it was added to the list, the object's name, and then the
    //setList containing the existing prefabs.
    private bool unitTestPresets()
    {
        GameObject[] tempList = Resources.LoadAll<GameObject>("StagePrefabs");

        if (unitTest != false)
        {
            foreach (Object temporary in tempList)
            {
                GameObject temp = (GameObject)temporary;
                if (temp.GetType() != null)
                {
                    print("Object exists. Adding to setList.");
                    setList.Add(temp);
                    print(temp.name + " added to setList.");
                }
                else
                    print("Object " + temp.name + " does not exist. Cannot add to setList.");
            }

            for (int index = 0; index < setList.Count; index++)
            {
                print(setList[index].name);
            }
            print("Test complete. Clearing setList.");
            setList.Clear();
        }
        return true;
    }
    //unitTestCustom().
    //Written by Catherine Stettler.
    //This function checks to see if a prefab exists in the resource folder and adds it to the setList. Mostly for debugging,
    //the function will print to the console if the object exists, that it was added to the list, the object's name, and then the
    //setList containing the existing prefabs.
    private bool unitTestCustom()
    {
        GameObject[] tempList = Resources.LoadAll<GameObject>("CustomStages");

        if (unitTest != false)
        {
            foreach (Object temporary in tempList)
            {
                GameObject temp = (GameObject)temporary;
                if (temp.GetType() != null)
                {
                    print("Object exists. Adding to setList.");
                    setList.Add(temp);
                    print(temp.name + " added to setList.");
                }
                else
                    print("Object " + temp.name + " does not exist. Cannot add to setList.");
            }

            for (int index = 0; index < setList.Count; index++)
            {
                print(setList[index].name);
            }
            print("Test complete. Clearing setList.");
            setList.Clear();
        }
        return true;
    }
    //unitTestInspector().
    //Written by Catherine Stettler.
    //This function checks to see if an item is assigned in the inspector. It returns false if
    //an item is unassigned, indicating that functionality may be impaired and printing a message to the console.
    private bool unitTestInspector()
    {
        if (unitTest == true)
        {
            if (clearSet == null)
                return false;
            if (deleteStage == null)
                return false;
            if (remObject == null)
                return false;
            if (props == null)
                return false;
            if (structures == null)
                return false;
            if (preset == null)
                return false;
            if (custom == null)
                return false;
        }
        return true;
    }
}
