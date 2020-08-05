//Ariel
//This converts the Scene class into serializable data that can be saved.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveableScene  
{
    


    //The following makes the List<GameObjects> saveable
    public string actor = "Actor", prop = "Props";
    public string[] saveSceneObjectList = new string[10];
    public List<int> prefabIndexesForObjects = new List<int>();
    public List<string> tagsForObjects = new List<string>();
    public int saveSceneID;

    public List<List<float[,]>> savePositions = new List<List<float[,]>>();
    public float[,] temp;
    public List<float[,]> saveStartPositions = new List<float[,]>();
    public List<float[,]> tempForSavePositions = new List<float[,]>();
    public List<float[,]> saveRotations = new List<float[,]>();

    //This function is a modified constructor class. A Saveable scene can be created when the MenuFunctions class creates
    //A scene.
    public SaveableScene(Scene scene, MenuFunctions current)
    {
        int i = 0;
        foreach (GameObject variable in scene.objects)
        {
            tagsForObjects.Add(variable.tag);

            if (variable.tag.Equals("Player"))
            {
                if (variable == current.actorDrop.prefabList[1])
                    prefabIndexesForObjects.Add(1);
                else
                    prefabIndexesForObjects.Add(2);

            }
            else if (variable.tag.Equals("Props"))
            {
                foreach(GameObject propVariable in current.propDrop.prefabList)
                {
                    if(propVariable == variable)
                    {
                        prefabIndexesForObjects.Add(current.propDrop.prefabList.IndexOf(propVariable));
                        break;
                    }
                }
            }
            
            //get scene ID
            saveSceneID = scene.getSceneID();

            //Start Position conversion
            temp = new float[scene.objects.Count,3];
            temp[i, 0] = scene.startPos[i].x;
            temp[i, 1] = scene.startPos[i].y;
            temp[i, 2] = scene.startPos[i].z;
            saveStartPositions.Add(temp);

            //Other position conversion
            
            foreach (List<Vector3> vec3List in scene.positions)
            {
                int j = 0; //j isn't putting the positions for each gameobject; why?
                Debug.Log("[SAVEABLE SCENES] scene.objects Count: " + scene.objects.Count);
                tempForSavePositions = new List<float[,]>();
                foreach (Vector3 vec3 in vec3List)
                {
                    if (j >= scene.objects.Count)
                        break;
                    
                    temp = new float[scene.objects.Count, 3];
                    temp[j, 0] = vec3.x;
                    temp[j, 1] = vec3.y;
                    temp[j, 2] = vec3.z;
                    Debug.Log("[SAVEABLE SCENES] scene.position for actor " + j +  ": (" + vec3.x + ", " + vec3.y + ", " + vec3.z + ") added!");
                    tempForSavePositions.Add(temp);
                    j++;
                }
                savePositions.Add(tempForSavePositions);
                Debug.Log("[SAVEABLE SCENES] scene.positions Count: " + scene.positions.Count);
            }

        }
    }
}
