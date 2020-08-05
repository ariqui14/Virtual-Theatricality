using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;


public class MenuFunctions : MonoBehaviour
{

    public GameObject nameDialogBox;//assigned in inspector
    public MenuDropdown actorDrop;//assigned in inspector
    public MenuDropdown propDrop;//assigned in inspector

    public GameObject selectionCirclePrefab;

    public Button teleportBttn;//assigned in inspector

    //Ariel
    //Helps to disable and enable playback buttons
    private Button disabledPlayback;
    private bool recordStarted = false;

    //Ariel
    public List<GameObject> objectList = new List<GameObject>();
    private List<string> actorOrProp = new List<string>();
    private List<int> BlueOrPink = new List<int>();
    private List<int> regPropIndexes = new List<int>();
    private List<Scene> SceneList = new List<Scene>();
    private List<SaveableScene> SaveableSceneList = new List<SaveableScene>();

    private List<Vector3> startPositions = new List<Vector3>();


    private GameObject selectedObject;
    private List<Coroutine> activeCoroutines = new List<Coroutine>();
    private List<GameObject> activeCoroutinesObjects = new List<GameObject>();
    private bool recordingEnd = false;
    private bool playbackHelper;
    public bool startCou;
    private int numRecordings = 0;
    public List<int> setPieceDropdownIndexes = new List<int>();

    public Font uiFont;
    //Ariel
    //The following button variables are for the playback system. As a scene is made, a new button will appear on the side for the user to click and choose which to playback
    bool scenesExist = false;
    public Dropdown dropdown; //assigned in inspector
    Scene selectedScene = new Scene();

    public GameObject buildUI; //Set in Inspector
    public List<GameObject> setList = new List<GameObject>();
    public buildFunctions buildFuncs; //set in Inspector

    private bool unitTest = true;
 
    //Daniel Thomas
    //Stops and Active coroutines and removes them from the coroutines list
    private void OnDisable()
    {
        while (activeCoroutines.Count != 0)
        {
            StopCoroutine(activeCoroutines[0]);
            activeCoroutines.RemoveAt(0);
            activeCoroutinesObjects.RemoveAt(0);
        }
    }

    //Daniel Thomas
    //Changes all font to what is assigned in the inspector when script loads
    private void Awake()
    {
        var textComponents = Resources.FindObjectsOfTypeAll<Text>();
        foreach (var component in textComponents)
            component.font =  uiFont;
    }

    //Daniel
    void Start()
    {
        //Unit testing. Will run only if unitTest = true.
        if (unitTestInspector() == true)
            print("All inspector elements assigned. No errors found.");
        else
            print("Inspector element unassigned. Functionality may be impaired.");
        if (unitTestRecord() == true)
            print("All record elements assigned. No errors found.");
        else
            print("Record element unassigned. Functionality may be impaired.");
        unitTestActors();
        unitTestProps();

        //Checks file name to load appropriate file on load
        if (File.Exists(Application.persistentDataPath + "/" + SessionData.myFileName + ".vt"))
        {
            try
            {
                LoadData();
            }
            catch
            {
                Debug.Log("Something went wrong when loading: MenuFunctions.Start()");
            }
        }

        //Ariel
        //Used for saving the static set list
        buildFuncs = buildUI.GetComponent<buildFunctions>();

    }

    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())//if mouse is over a UI menu, return
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        //Ariel & Daniel
        //The following makes all the playback buttons except for "Start Recording" and "Next" false if Start Recording has not been clicked, and viceversa.
        if (recordStarted)
        {
            DisableAllButtons();
        }
        else
        {
            EnableAllButtons();
        }

        //Ariel
        //The following evaluates the dropdown index and choose that Scene as the user's desired playback
        if (dropdown.value > 0)
        {
            foreach (Scene variable in SceneList)
            {
                if (variable.getSceneID() == dropdown.value)
                {
                    selectedScene = new Scene();
                    selectedScene = variable;
                    break;
                }
            }
        }

        //Ariel
        //Makes the play button interactable or not based on if a scene is selected in the dropdown
        if (dropdown.value == 0)
        {
            GameObject.Find("PlayRecording").GetComponent<Button>().interactable = false;
        }
        else
        {
            GameObject.Find("PlayRecording").GetComponent<Button>().interactable = true;

        }


        //Daniel
        //Handles all object selection, activates selection circles of selected object, enables appropriate buttons for the selected object
        //left click of mouse, uses raycast
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform != null && (hit.transform.gameObject.CompareTag("Player") || hit.transform.gameObject.CompareTag("Props")))
                {
                    //turn off selection circle of previously selected object
                    if(selectedObject != null)
                        selectedObject.transform.Find("SelectionCylinder(Clone)").gameObject.SetActive(false);

                    teleportBttn.interactable = true;
                    selectedObject = objectList.Find(myInput => myInput == hit.transform.gameObject);
                    selectedObject.transform.Find("SelectionCylinder(Clone)").gameObject.SetActive(true);
                    if (hit.transform.gameObject.CompareTag("Player"))
                    {
                        GameObject.Find("RenameActorButton").GetComponent<Button>().interactable = true;
                    }
                    else if (hit.transform.gameObject.CompareTag("Props"))
                    {

                    }
                }
                else
                {
                    if (selectedObject != null)
                        selectedObject.transform.Find("SelectionCylinder(Clone)").gameObject.SetActive(false);
                    teleportBttn.interactable = false;
                    GameObject.Find("RenameActorButton").GetComponent<Button>().interactable = false;
                    deselectObject();
                }
            }
        }

        //Daniel
        //Below is right click to move an object to a certain position on the stage
        if (Input.GetMouseButtonDown(1) && selectedObject != null)
        {
            if (Physics.Raycast(ray, out hit) && hit.transform != null && hit.transform.gameObject.CompareTag("Ground"))
            {
                AddMoveCoroutineOnSelectedObject(hit.point);
            }
        }

        //Daniel
        //Object rotation with scroll wheel
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
        }

    }

    //Ariel
    //Getters for private variables
    public List<GameObject> GetObjectList() {
        return objectList;
    }

    public GameObject GetSelectedObject()
    {
        return selectedObject;
    }

    public List<int> GetBlueOrPinkList()
    {
        return BlueOrPink;
    }

    public List<SaveableScene> GetSaveableScenesList()
    {
        return SaveableSceneList;
    }

    public List<string> GetActorOrPropList()
    {
        return actorOrProp;
    }

    //Daniel
    //Adds a move to the coroutine list when an object moves. Meant to keep track of all coroutines
    public void AddMoveCoroutineOnSelectedObject(Vector3 DesiredPos)
    {
        activeCoroutines.Add(StartCoroutine(MoveObject(selectedObject, DesiredPos)));
    }

    //Daniel
    //Returns the game back to the title menu and changes data back to null that is file specific
    public void MainMenuButton_Click(bool saveTheatre)
    {
        if (saveTheatre)
        {
            SaveData();
        }
        SessionData.selectedStageType = null;
        SessionData.myFileName = null;
        SceneManager.LoadScene("Title_Screen");
    }
    
    //Daniel
    //Function that starts the coroutine for adding an object.
    public void AddObjectFunction(GameObject newObject)
    {
        StartCoroutine(WaitForAnswerAddObject(newObject));
    }

    //Daniel
    //Function that starts the coroutine for removing an object.
    public void RemoveObjectFunction()
    {
        StartCoroutine(WaitForAnswerDestroy());
    }

    //Daniel
    //Function that starts the coroutine for teleporting an object.
    public void TeleportObjectFunction()
    {
        StartCoroutine(TeleportObject(selectedObject));

    }


    //Ariel
    //Function to call SaveSystem class and serialize stage data (objects, positions, prefabs, etc.)
    public void SaveData()
    {
        SaveSystem.SaveFunction(this);
    }

    //Daniel
    //Function to call SaveSystem class via pause menu.
    public void SaveDataViaPause(MenuFunctions myMenu)
    {
        SaveSystem.SaveFunction(myMenu);
    }


    //Ariel
    //Following variables aid in playback functions
    List<int> recordingsInt = new List<int>();
    int x = 0;
    int recordingIndex = 0;
    List<Vector3> startpositions = new List<Vector3>();
    List<Vector3> nextpositions = new List<Vector3>();
    List<Vector3> endpositions = new List<Vector3>();

    List<GameObject> startObjectList = new List<GameObject>();
    List<GameObject> nextObjectList = new List<GameObject>();
    List<GameObject> endObjectList = new List<GameObject>();
    List<Quaternion> objectRotationsForPlayback = new List<Quaternion>();


    //Ariel 
    //This function starts the recording, it creates a list of the current objects on the stage and takes note of their current starting positions.
    //Also keeps track of the number of movements the objects make in the scene.
    public void StartRecording_Click()
    {
        recordStarted = true;
        GameObject.Find("StartRecord").GetComponent<Button>().interactable = false;

       // Debug.Log("Starting a recording.");
        //Save the GameObjects at their start positions
        startObjectList = new List<GameObject>();
        startpositions = new List<Vector3>();
        foreach (GameObject variable in objectList)
        {
            startObjectList.Add(variable);
            objectRotationsForPlayback.Add(variable.transform.rotation);
            startpositions.Add(variable.transform.position);

        }

        //The following is to keep track of the number of recordings and movements in the current
        //saved scene for debugging purposes.
     //   Debug.Log("Starting a recording.");
        recordingsInt.Add(x);
       // Debug.Log("The number of scenes in this recording is: " + recordingsInt.Count);
        //Debug.Log("The number of movements in the recording is " + x);


    }

    //The following variable is a list of a list of positions for the dolls to move to during playback
    List<List<Vector3>> nextPositions = new List<List<Vector3>>();

    //Ariel
    //This function, when the user has moved the object to the next position, saves that movement. They can repeat this again and again, until pressing "End Recording"
    private void Next_Click()
    {

        nextpositions = new List<Vector3>();

      //  Debug.Log("Adding a  movement to the recording.");

        //If the user clicks next, that means that they want to save the positions of the game objects
        //As this next movement. A number of GameObject.Count positions are added to the list of
        //Positions called "currentlyRecording"
        foreach (GameObject variable in objectList)
        {
            nextObjectList.Add(variable);
            nextpositions.Add(variable.transform.position);

        }

        nextPositions.Add(nextpositions);
        //The following is to keep track of the number of recordings and movements in the current
        //saved scene for debugging purposes.
        x++;
        recordingsInt[recordingIndex] = x;
     //   Debug.Log("The number of scenes in this recording is: " + recordingsInt.Count);
       // Debug.Log("The number of movements in the recording is " + x);

    }

    //Ariel
    //This function ends the recording, and saves all of the movements the user has recorded with the "Next" function, and places them into a Scene.
    private void EndRecording_Click()
    {
        recordStarted = false;
        numRecordings++;
        endpositions = new List<Vector3>();
        GameObject.Find("StartRecord").GetComponent<Button>().interactable = true;

        /*When the user clicks "End Recording", the list of positions is placed 
         *into a list of list of positions. The scene is over, and the user will now
         *build the next scene's starting positions.
         */
        foreach (GameObject variable in objectList)
        {
            endObjectList.Add(variable);
            endpositions.Add(variable.transform.position);

        }

        nextPositions.Add(endpositions);
        Dropdown dropdown = GameObject.Find("SceneDropdown").GetComponent<Dropdown>();
        List<Dropdown.OptionData> m_Messages = new List<Dropdown.OptionData>();
        string newOptionString = "Scene " + numRecordings;
        Dropdown.OptionData newOption = new Dropdown.OptionData();

        List<Dropdown.OptionData> newOptionList = new List<Dropdown.OptionData>();
        newOptionList.Add(newOption);
        newOption.text = newOptionString;
        dropdown.AddOptions(newOptionList);

        //The following creates an option in the Scene Dropdown Menu
        //The following is to keep track of the number of recordings and movements in the current
        //saved scene for debugging purposes.
        List<Dropdown.OptionData> m_messages = new List<Dropdown.OptionData>();
        Scene newScene = new Scene();
        newScene.CreateScene(nextPositions, startpositions);

        foreach(GameObject variable in startObjectList)
        {
            newScene.addToObjectsInScene(variable);
            newScene.setObjectsInScenetoInactive();
        }

        foreach(Quaternion anotherVar in objectRotationsForPlayback)
        {
            newScene.addQuaternion(anotherVar);
        }
        newScene.setSceneID(numRecordings);
        SceneList.Add(newScene);
        SaveableScene tempSaveableScene = new SaveableScene(newScene, this);
        SaveableSceneList.Add(tempSaveableScene);
       
        nextPositions = new List<List<Vector3>>();
        recordingIndex++;
        x = 0;


    }
    bool PlayRecordCoroutineBool = false;

//    Ariel
//    This is the actual coroutine that plays the selected Scene from the dropdown menu, movement by movmeent. It loads all of the objects in the scene at their start positions.
    IEnumerator PlayRecordCoroutine(Scene scene)
    {
        DisableAllButtons();
        DisableStage();
        PlayRecordCoroutineBool = true;
        List<List<Vector3>> positions = scene.positions;
        List<GameObject> gameObjects = scene.objects;
        Vector3 curPos;
        Vector3 lastPos;

        for(int startIndex = 0; startIndex < gameObjects.Count; startIndex++)
        {   gameObjects[startIndex].SetActive(true);
            gameObjects[startIndex].transform.position = selectedScene.startPos[startIndex];
            gameObjects[startIndex].transform.rotation = selectedScene.rotationsForObjects[startIndex];
          
            gameObjects[startIndex].transform.Find("SelectionCylinder(Clone)").gameObject.SetActive(false);
        }
     
        GameObject objectMoving;
        foreach (List<Vector3> corouPos in positions) {
            int startTemp1 = 0;
            foreach (Vector3 vec3 in corouPos)
            {
             //   Debug.Log("[PLAYBACK] Number of GameObjects in the Scene is " + gameObjects.Count);
                objectMoving = gameObjects[startTemp1];

                    Debug.Log("[PLAYBACK] Actor " + startTemp1 + " moving to " + vec3);
                    activeCoroutines.Add(StartCoroutine(MoveObject(objectMoving, vec3)));

                if (startTemp1 < gameObjects.Count-1)
                    startTemp1++;
                else
                    startTemp1 = 0;

            }

            //As the objects are moving, wait until they hit their marks before moving onto next position
            for (int startTemp = 0; startTemp < gameObjects.Count; startTemp++)
            {
                objectMoving = gameObjects[startTemp];
                curPos = objectMoving.transform.position;
                lastPos = objectMoving.transform.position;

                int x = 0;

                while(x == 0)
                {
                    curPos = objectMoving.transform.position;
                    if (curPos == lastPos)
                    {
                        yield return new WaitForSeconds(1);
                        break;
                    }
                    else
                   
                    {
                        lastPos = curPos;
                    }
                }

            }     
           
        }

        yield return new WaitForSeconds(1);

        foreach (GameObject variable in gameObjects)
        {

            variable.SetActive(false);

        }

        for(int u = 0; u < tempPlayRec.Count; u++)
        {
            tempPlayRec[u].SetActive(true);
        }
    
        yield return null;


    }

    List<GameObject> tempPlayRec;
    List<Vector3> tempPlayRecPos = new List<Vector3>();

    //Ariel
    //This is the function that activates the Playback Coroutine above.
    public void PlayRecording_Click()
    {
        Debug.Log("selectedScene.positions.Count: " + selectedScene.positions.Count);
            foreach(List<Vector3> pos in selectedScene.positions)
        {
            foreach (Vector3 pos1 in pos)
            {
                Debug.Log("Position: " + pos1);
            }
        }

        SaveData();

        tempPlayRec = objectList;

        foreach (GameObject variable in objectList)
        {
            tempPlayRecPos.Add(variable.transform.position);
            variable.SetActive(false);
        }

        List<GameObject> tempObject = new List<GameObject>();
        //---------------------------Move object position to the nextpositions

        StartCoroutine(PlayRecordCoroutine(selectedScene));

    }

    //Ariel
    //The following loads a save file .
    public void LoadData()
    {
        ClearStage();
        SceneList = new List<Scene>();
        SaveableSceneList = new List<SaveableScene>();

        //------------------------------------------Load the saved actors and props
        Save data = SaveSystem.loadData();

        int LoadIndex = 0;
        while (LoadIndex <= data.numObjectsSave - 1)
        {
            float[] loadPosArray = new float[3];
            loadPosArray[0] = data.vec3PositionsAsFloat[LoadIndex, 0];
            loadPosArray[1] = data.vec3PositionsAsFloat[LoadIndex, 1];
            loadPosArray[2] = data.vec3PositionsAsFloat[LoadIndex, 2];
            //Debug.Log("Load Data ActorOrProp Object: " + data.actorOrProp[LoadIndex]);
            if (data.actorOrProp[LoadIndex].Equals("Player"))
            {
                if (data.BlueOrPink[LoadIndex] == 0)
                {
                    GameObject newObject = Instantiate(actorDrop.prefabList[0], new Vector3(loadPosArray[0], loadPosArray[1], loadPosArray[2]),
                        actorDrop.prefabList[0].transform.rotation);

                    //------------------------ Add selection circle to object
                    GameObject newSelectionCircle = Instantiate(selectionCirclePrefab);
                    newSelectionCircle.transform.SetParent(newObject.transform);
                    newSelectionCircle.SetActive(false);
                    //------------------------

                    objectList.Add(newObject);
                    actorOrProp.Add("Actor");
                    BlueOrPink.Add(0);
                }
                else if (data.BlueOrPink[LoadIndex] == 1)
                {
                    GameObject newObject = Instantiate(actorDrop.prefabList[1], new Vector3(loadPosArray[0], loadPosArray[1], loadPosArray[2]),
                        actorDrop.prefabList[1].transform.rotation);

                    //------------------------ Add selection circle to object
                    GameObject newSelectionCircle = Instantiate(selectionCirclePrefab);
                    newSelectionCircle.transform.SetParent(newObject.transform);
                    newSelectionCircle.SetActive(false);
                    //------------------------

                    objectList.Add(newObject);
                    actorOrProp.Add("Actor");
                    BlueOrPink.Add(1);
                }
            }
            else if (data.actorOrProp[LoadIndex].Equals("Props"))
            {
                GameObject newObject = Instantiate(propDrop.prefabList[0], new Vector3(loadPosArray[0], loadPosArray[1], loadPosArray[2]),
                    propDrop.prefabList[0].transform.rotation);

                //------------------------ Add selection circle to object
                GameObject newSelectionCircle = Instantiate(selectionCirclePrefab);
                newSelectionCircle.transform.SetParent(newObject.transform);
                newSelectionCircle.SetActive(false);

                //------------------------

                objectList.Add(newObject);
                actorOrProp.Add("Props");
                BlueOrPink.Add(-1);

            }
            else
            {
                Debug.Log("Instantiating a null Game Object?");
            }

            LoadIndex++;
        }


        //--------------------------Reinstantiating the SceneList
        SaveableSceneList = data.scenes;

        foreach (SaveableScene scene in SaveableSceneList)
        {
            Scene tempScene = new Scene();
            tempScene.saveableScenesIntoScenes(scene, this);
            int actorCount = 0;
            foreach(List<Vector3> pos in tempScene.positions)
            {
                foreach (Vector3 pos2 in pos)
                {
                   Debug.Log("[LOAD] Actor " + actorCount + " Scene Pos: " + pos2);
                    if (actorCount >= tempScene.objects.Count - 1)
                    {
                        actorCount = 0;
                    }
                    else
                        actorCount++;
                }             
            }

            actorCount = 0;
            foreach(List <float[,]> pos in scene.savePositions)
            {
                foreach (float[,] pos2 in pos)
                {
                    Debug.Log("[LOAD] Actor " + actorCount  +
                        " Saveable Scene Pos: (" + pos2[0,0] + ", " + pos2[0,1] + ", " + pos2[0,2] + ")");
                    if (actorCount >= tempScene.objects.Count - 1)
                    {
                        actorCount = 0;
                    }
                    else
                        actorCount++;
                }
                
            }
            SceneList.Add(tempScene);
        }

        //-------------------------------------Resetting the dropdown
        dropdown = GameObject.Find("SceneDropdown").GetComponent<Dropdown>();
        dropdown.ClearOptions();
        List<string> dropStrings = new List<string>();
        
        dropStrings.Add("No Scene");

        for (int dropIndex =0; dropIndex< SaveableSceneList.Count; dropIndex++)
        {
            dropStrings.Add("Scene " + (dropIndex + 1));
        }

        dropdown.AddOptions(dropStrings);

        //-------------------------------------Reinstantiating SetList
        setList = new List<GameObject>();
        setPieceDropdownIndexes = new List<int>();
        int setListLoadIndex = 0;
        while (setListLoadIndex < data.numSetList)
        {
            float[] setListPosArray = new float[3];
            setListPosArray[0] = data.vec3PosForSetPieces[setListLoadIndex, 0];
            setListPosArray[1] = data.vec3PosForSetPieces[setListLoadIndex, 1];
            setListPosArray[2] = data.vec3PosForSetPieces[setListLoadIndex, 2];

            GameObject newObject = Instantiate(buildFuncs.props.prefabList[data.setPieceDropdownIndexes[setListLoadIndex]],
                new Vector3(setListPosArray[0], setListPosArray[1], setListPosArray[2]), Quaternion.identity);

            setList.Add(newObject);
            setPieceDropdownIndexes.Add(setListLoadIndex);
        }

    }

    //Daniel
    //This coroutine removes an object from the scene and also removes from our object data lists
    IEnumerator WaitForAnswerDestroy()
    {
        for (; ; )
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform != null && (hit.transform.gameObject.CompareTag("Player") || hit.transform.gameObject.CompareTag("Props")))
                    {
                        int tempIndex = objectList.FindIndex(myObject => myObject == hit.transform.gameObject);

                        actorOrProp.RemoveAt(tempIndex);
                        BlueOrPink.RemoveAt(tempIndex);
                        objectList.RemoveAt(tempIndex);
                        Destroy(hit.transform.gameObject);
                        selectedObject = null;
                    }

                }
                break;
            }
            yield return null;
        }
    }

    //Ariel
    //Modified by Daniel
    //The purpose of this function is to disable/enable all buttons until playback is finished
    private void DisableAllButtons()
    {

        //Buttons Part of Toolbar UI
        GameObject.Find("RemoveObjectButton").GetComponent<Button>().interactable = false;
        GameObject.Find("TeleportObjectButton").GetComponent<Button>().interactable = false;
        GameObject.Find("StartRecord").GetComponent<Button>().interactable = false;
        GameObject.Find("ClearStageBttn").GetComponent<Button>().interactable = false;
        GameObject.Find("EndRecord").GetComponent<Button>().interactable = true;////////////////////////////////
        GameObject.Find("RenameActorButton").GetComponent<Button>().interactable = false;
        GameObject.Find("Next").GetComponent<Button>().interactable = true;///////////////////////////////////////
        GameObject.Find("PlayRecording").GetComponent<Button>().interactable = false;
        GameObject.Find("AddPropDropdown").GetComponent<Dropdown>().interactable = false;
        GameObject.Find("AddActorDropdown").GetComponent<Dropdown>().interactable = false;

        //Buttons Part of Swap UI
        GameObject.Find("Build Set").GetComponent<Button>().interactable = false;
        GameObject.Find("Block Set").GetComponent<Button>().interactable = false;
        GameObject.Find("Standard Camera").GetComponent<Button>().interactable = false;
        GameObject.Find("Top-Down Camera").GetComponent<Button>().interactable = false;


    }
    private void EnableAllButtons()
    {
        //Buttons Part of Toolbar UI
        GameObject.Find("RemoveObjectButton").GetComponent<Button>().interactable = true;

        if(selectedObject != null && (selectedObject.CompareTag("Player") || selectedObject.CompareTag("Prop")))
            GameObject.Find("TeleportObjectButton").GetComponent<Button>().interactable = true;

        GameObject.Find("StartRecord").GetComponent<Button>().interactable = true;
        GameObject.Find("ClearStageBttn").GetComponent<Button>().interactable = true;
        GameObject.Find("EndRecord").GetComponent<Button>().interactable = false;///////////////////////////////

        if(selectedObject != null && selectedObject.CompareTag("Player"))
            GameObject.Find("RenameActorButton").GetComponent<Button>().interactable = true;

        GameObject.Find("Next").GetComponent<Button>().interactable = false;///////////////////////////////////
        GameObject.Find("PlayRecording").GetComponent<Button>().interactable = true;
        GameObject.Find("AddPropDropdown").GetComponent<Dropdown>().interactable = true;
        GameObject.Find("AddActorDropdown").GetComponent<Dropdown>().interactable = true;

        //Buttons Part of Swap UI
        GameObject.Find("Build Set").GetComponent<Button>().interactable = true;
        GameObject.Find("Block Set").GetComponent<Button>().interactable = true;
        GameObject.Find("Standard Camera").GetComponent<Button>().interactable = true;
        GameObject.Find("Top-Down Camera").GetComponent<Button>().interactable = true;

    }

    //Daniel
    //This coroutine adds an object to the stage and to our object data lists
    IEnumerator WaitForAnswerAddObject(GameObject newObject)
    {
        for (; ; )
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform != null && hit.transform.gameObject.CompareTag("Ground"))
                    {
                        GameObject temp = Instantiate(newObject, hit.point, newObject.transform.rotation);
                        GameObject newSelectionCircle = Instantiate(selectionCirclePrefab);
                        newSelectionCircle.transform.SetParent(temp.transform);
                        newSelectionCircle.SetActive(false);


                        if (newObject.transform.gameObject.CompareTag("Player"))
                        {
                            objectList.Add(temp);
                            actorOrProp.Add("Actor");
                            if (newObject == actorDrop.prefabList[0])
                                BlueOrPink.Add(0);
                            else if (newObject == actorDrop.prefabList[1])
                                BlueOrPink.Add(1);
                        }
                        else if (newObject.transform.gameObject.CompareTag("Props"))
                        {
                            objectList.Add(temp);
                            actorOrProp.Add("Props");
                            
                            BlueOrPink.Add(-1);
                        }
                    }
                }
                break;
            }
            yield return null;
        }
    }

    //Daniel
    //This coroutine teleports objects from one part of the stage to another with a right click, not sliding them like the other move coroutine
    IEnumerator TeleportObject(GameObject tempObject)
    {
        for (; ; )
        {
            //select new position
            if (Input.GetMouseButtonDown(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform != null && hit.transform.gameObject.CompareTag("Ground"))
                    {
                        tempObject.transform.position = hit.point;
                    }
                }
                break;
            }
            else if (selectedObject != tempObject)
            {
                break;//if the object to telelport is deselected before teleport is done, teleport will cancel
            }

            yield return null;
        }
    }

    bool objectsMoving = false;

    //Daniel
    //This coroutine moves objects either by sliding (props) or walking (actors)
    IEnumerator MoveObject(GameObject objectToMove, Vector3 newPosition)
    {
        objectsMoving = true;
        int tempIndex = -1;
        if (activeCoroutinesObjects.Exists(myInput => myInput == objectToMove))
        {
            tempIndex = activeCoroutinesObjects.FindIndex(myInput => myInput == objectToMove);
            StopCoroutine(activeCoroutines[tempIndex]);
            activeCoroutines.RemoveAt(tempIndex);
            activeCoroutinesObjects.RemoveAt(tempIndex);
        }

        activeCoroutinesObjects.Add(objectToMove);

        newPosition.y = objectToMove.transform.position.y;
        int callOnce = 0;
        for (; ; )
        {
            if (Vector3.Distance(newPosition, objectToMove.transform.position) >= 0.1f)
            {
                if (objectToMove.CompareTag("Player"))
                {
                    objectToMove.transform.LookAt(newPosition);
                    if (callOnce == 0)
                    {
                        objectToMove.GetComponent<Animator>().Play("Locomotion");
                        objectToMove.GetComponent<Animator>().SetFloat("Speed", .5f);
                        callOnce++;
                    }
                }
                else if (objectToMove.CompareTag("Props"))
                    objectToMove.transform.position = Vector3.MoveTowards(objectToMove.transform.position, newPosition, Time.deltaTime * 2.0f);
            }
            else
            {
                if (objectToMove.CompareTag("Player"))
                {
                    objectToMove.GetComponent<Animator>().Play("Idle");
                    objectToMove.GetComponent<Animator>().SetFloat("Speed", 0f);

                }

                tempIndex = activeCoroutinesObjects.FindIndex(myInput => myInput == objectToMove);
                yield return new WaitForSeconds(2);
                activeCoroutines.RemoveAt(tempIndex);
                activeCoroutinesObjects.RemoveAt(tempIndex);
                break;
            }
            yield return null;
        }


    }

    //Ariel/Daniel
    //The following clears the stage of all objects
    public void ClearStage()
    {
        //Needs to parse out data from object's Game Object list, like the male/female actors
        //Delete all active actors and props
        int deleteIndex = 0;

        while (objectList.Count > 0)
        {
            GameObject temp = objectList[deleteIndex];
            Destroy(temp);
            objectList.RemoveAt(deleteIndex);

            actorOrProp.RemoveAt(deleteIndex);
            BlueOrPink.RemoveAt(deleteIndex);
        }

        //End all corountines on load
        int myTempIndex = 0;
        while (activeCoroutines.Count > 0)
        {
            StopCoroutine(activeCoroutines[myTempIndex]);
            activeCoroutines.RemoveAt(myTempIndex);
            activeCoroutinesObjects.RemoveAt(myTempIndex);
        }
    }

    //Ariel
    //Set all actors and props to inactive
    public void DisableStage()
    {
        foreach (GameObject variable in objectList)
        {

            variable.SetActive(false);

        }
    }

    //Set all actors and props to active
    public void EnableStage()
    {
        foreach (GameObject variable in objectList)
        {

            variable.SetActive(true);

        }

    }

    //Daniel
    //The following three functions allow the user to rename an actor
    public void RenameActorOpen_Click()
    {
        nameDialogBox.SetActive(true);
    }

    public void RenameActorAccept_Click()
    {
        selectedObject.transform.Find("Player Name Canvas").Find("Player Name Text").GetComponent<TMPro.TextMeshProUGUI>().text = nameDialogBox.GetComponentInChildren<InputField>().text;
        nameDialogBox.GetComponentInChildren<InputField>().text = "";
        nameDialogBox.SetActive(false);
    }

    public void RenameActorCancel_Click()
    {
        nameDialogBox.GetComponentInChildren<InputField>().text = "";
        nameDialogBox.SetActive(false);
    }

    //Daniel
    //The following function deselects the currently selected object.
    private void deselectObject()
    {
        selectedObject = null;
    }

    //Unit tests

    //unitTestProps().
    //Written by Catherine Stettler.
    //This function checks to see if a prefab exists in the resource folder and adds it to the objectList. Mostly for debugging,
    //the function will print to the console if the object exists, that it was added to the list, the object's name, and then the
    //objectList containing the existing prefabs.
    private bool unitTestProps()
    {
        GameObject[] tempList = Resources.LoadAll<GameObject>("PropPrefabs");

        if (unitTest != false)
        {
            foreach (Object temporary in tempList)
            {
                GameObject temp = (GameObject)temporary;
                if (temp.GetType() != null)
                {
                    print("Object exists. Adding to objectList.");
                    objectList.Add(temp);
                    print(temp.name + " added to objectList.");
                }
                else
                    print("Object " + temp.name + " does not exist. Cannot add to objectList.");
            }

            for (int index = 0; index < objectList.Count; index++)
            {
                print(objectList[index].name);
            }
            print("Test complete. Clearing objectList.");
            objectList.Clear();
        }
        return true;
    }
    //unitTestActors().
    //Written by Catherine Stettler.
    //This function checks to see if a prefab exists in the resource folder and adds it to the objectList. Mostly for debugging,
    //the function will print to the console if the object exists, that it was added to the list, the object's name, and then the
    //objectList containing the existing prefabs.
    private bool unitTestActors()
    {
        GameObject[] tempList = Resources.LoadAll<GameObject>("ActorPrefabs");

        if (unitTest != false)
        {
            foreach (Object temporary in tempList)
            {
                GameObject temp = (GameObject)temporary;
                if (temp.GetType() != null)
                {
                    print("Object exists. Adding to objectList.");
                    objectList.Add(temp);
                    print(temp.name + " added to objectList.");
                }
                else
                    print("Object " + temp.name + " does not exist. Cannot add to objectList.");
            }

            for (int index = 0; index < objectList.Count; index++)
            {
                print(objectList[index].name);
            }
            print("Test complete. Clearing objectList.");
            objectList.Clear();
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
            if (actorDrop == null)
                return false;
            if (propDrop == null)
                return false;
            if (teleportBttn == null)
                return false;
            if (dropdown == null)
                return false;
            if (buildUI == null)
                return false;
            if (buildFuncs == null)
                return false;
            if (nameDialogBox == null)
                return false;
        }
        return true;
    }
    //unitTestRecord().
    //Written by Catherine Stettler.
    //This function checks to see if an item is assigned for the record functionality. It returns false if
    //an item is unassigned, indicating that functionality may be impaired and printing a message to the console.
    private bool unitTestRecord()
    {
        if (unitTest == true)
        {
            if (GameObject.Find("PlayRecording") == null)
                return false;
            if (GameObject.Find("EndRecord") == null)
                return false;
            if (GameObject.Find("StartRecord") == null)
                return false;
            if (GameObject.Find("Next") == null)
                return false;
        }
        return true;
    }
}
;