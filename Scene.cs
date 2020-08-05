//Ariel
//This class creates recorded movements of the actors and props and categorizes them into "Scenes"

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene : Component
{
    public List<List<Vector3>> positions = new List<List<Vector3>>();
    public List<GameObject> objects = new List<GameObject>();
    public List<Vector3> startPos = new List<Vector3>();
    public List<Quaternion> rotationsForObjects = new List<Quaternion>();

    private int SceneID;

    //The following is a Constructor for the Scene class.
    public Scene()
    {
        positions = new List<List<Vector3>>();
        objects = new List<GameObject>();
        startPos = new List<Vector3>();
        rotationsForObjects = new List<Quaternion>();
    }

    //The following sets the positions and the start position of the objects in the scene class
    public void CreateScene(List<List<Vector3>> positionList, List<Vector3> startPosIn)
    {
        startPos = startPosIn;
        positions = positionList;
    }

    //The following gets/sets the scene ID for each scene to be identified in the scene dropdown
    public void setSceneID(int number)
    {
        SceneID = number;
    }
    public int getSceneID()
    {
        return SceneID;
    }

    //Thie function saves the rotation of the game objects in the scene's start positions.
    public void addQuaternion(Quaternion rotation)
    {
        rotationsForObjects.Add(rotation);
    }

    
    //This functions adds a gameObject to the Scene's object List
    public void addToObjectsInScene(GameObject variable)
    {
        GameObject temp = Instantiate(variable, variable.transform.position, Quaternion.identity);
        objects.Add(temp);
    }

    //This functions makes all the objects in the scene inactive.
    public void setObjectsInScenetoInactive()
    {
        foreach (GameObject variable in objects){
            variable.SetActive(false);
        }
    }

    //This is used in the Load function, it takes the list of SaveableScenes from the JSON file,
    //and repurposes the serialized data to refill the SceneList in the program.
    public void saveableScenesIntoScenes(SaveableScene scene, MenuFunctions current)
    {
        
        //getID
        SceneID = scene.saveSceneID;

        //get the start positions
        int start = 0;
        foreach(float[,] floatVec3s in scene.saveStartPositions)
        {
            Vector3 tempVec3 = new Vector3(floatVec3s[start,0], floatVec3s[start, 1], floatVec3s[start, 1]);
            startPos.Add(tempVec3);
            start++;
        }


        //get the positions
        
        foreach(List<float[,]> floatVec3Nexts in scene.savePositions)
        {
            List<Vector3> tempListToPutInNextPos = new List<Vector3>();
            //Debug to check the positions of the saveable Scene
            int objectIndex = 0;
            foreach (float[,] pos2 in floatVec3Nexts)
            {
                Debug.Log("[SCENE CLASS] Actor " + objectIndex + " Saveable Scene Pos: (" + pos2[0, 0] + ", " + pos2[0, 1] + ", " + pos2[0, 2] + ")");

                Vector3 tempVec = new Vector3(pos2[objectIndex, 0], pos2[objectIndex, 1], pos2[objectIndex, 2]);
                Debug.Log("[SCENE CLASS] Actor " + objectIndex + " Scene Pos: " + tempVec);


                tempListToPutInNextPos.Add(tempVec);
                if (objectIndex > objects.Count)
                    objectIndex = 0;
                else
                    objectIndex++;
            }
            
            positions.Add(tempListToPutInNextPos);
        }

        Debug.Log("positions.Count" + positions.Count);

        //get the objects
        int tempPrefabCheck = 0;
        foreach(string tagString in scene.tagsForObjects)
        {
            

            if (tagString.Equals("Player"))
            {
                GameObject buildObject = Instantiate(current.actorDrop.prefabList[scene.prefabIndexesForObjects[tempPrefabCheck]], startPos[tempPrefabCheck], Quaternion.identity);
                objects.Add(buildObject);
                buildObject.SetActive(false);

            }
            else if (tagString.Equals("Props"))
            {
                GameObject buildObject = Instantiate(current.propDrop.prefabList[scene.prefabIndexesForObjects[tempPrefabCheck]], startPos[tempPrefabCheck], Quaternion.identity);
                objects.Add(buildObject);
                buildObject.SetActive(false);
            }
     

            tempPrefabCheck++;
        }
    }
}
