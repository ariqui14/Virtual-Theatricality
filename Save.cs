//Ariel

//This class converts the data from the program into serializable data that can be stored in a .Json file.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


[System.Serializable]
public class Save 
{

    public int numObjectsSave = 0, numSetList = 0;
    public string actor = "Actor", prop = "Props";
    public string userNameSave, theatreNameSave;
    public string[] objectListSave;
    private float[] returnArray = new float[3];
    public List<string> actorOrProp = new List<string>();
    public List<string> actorOrPropSetList = new List<string>();
    public List<int> BlueOrPink = new List<int>();
    public float[,] vec3PositionsAsFloat;
    public float[] test = new float[3];
    public string stageType;//To determine what stage to load
    public List<SaveableScene> scenes = new List<SaveableScene>();

    public List<int> setPieceDropdownIndexes = new List<int>();
    public List<int> regPropDropdownIndexes = new List<int>();
    public float[,] vec3PosForSetPieces;




    
    //This is a modified constructor of the Save class. When a Save object is created, it takes the
    //information from the MenuFunctions class and serializes it.
    public Save(MenuFunctions currentMenuFuncs) {
        
        vec3PositionsAsFloat = new float[currentMenuFuncs.objectList.Count, 3];
        numObjectsSave = currentMenuFuncs.objectList.Count;
        BlueOrPink = currentMenuFuncs.GetBlueOrPinkList();
        theatreNameSave = SessionData.myFileName;
        stageType = SessionData.selectedStageType;
        setPieceDropdownIndexes = currentMenuFuncs.setPieceDropdownIndexes;
        scenes = currentMenuFuncs.GetSaveableScenesList();
        int i = 0, j=0;


        //Following should convert all Vector3 objects into float arrays to be serialized.
        while (i < currentMenuFuncs.objectList.Count) {
            actorOrProp.Add(currentMenuFuncs.objectList[i].tag);
            vec3PositionsAsFloat[i, 0] = currentMenuFuncs.objectList[i].transform.position.x;
            vec3PositionsAsFloat[i, 1] = currentMenuFuncs.objectList[i].transform.position.y;
            vec3PositionsAsFloat[i, 2] = currentMenuFuncs.objectList[i].transform.position.z;
            i++;
        }

        //Following to convert static set pieces as serializable
        while (j < currentMenuFuncs.setList.Count)
        {
            actorOrPropSetList.Add(currentMenuFuncs.setList[j].tag);
            vec3PosForSetPieces[j, 0] = currentMenuFuncs.setList[j].transform.position.x;
            vec3PosForSetPieces[j, 1] = currentMenuFuncs.setList[j].transform.position.y;
            vec3PosForSetPieces[j, 2] = currentMenuFuncs.setList[j].transform.position.z;

            j++;
        }
        numSetList = currentMenuFuncs.setList.Count;
    }


}



