//Ariel
//The following takes the save data and serializes the code and creates a .JSON file

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveSystem
{
    public static void SaveFunction(MenuFunctions current)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path;
        //Custom File Name
        Debug.Log("File Name2: " + SessionData.myFileName);

        //Daniel
        if (SessionData.myFileName == null)
        {
            path = Application.persistentDataPath + "/NewTheatre.vt";
            SessionData.myFileName = "NewTheatre";
            int tempInt = 1;
            while (File.Exists(path))
            {
                path = Application.persistentDataPath + "/NewTheatre" + tempInt + ".vt";
                SessionData.myFileName = "NewTheatre" + tempInt;
                tempInt++;
            }
        }

        path = Application.persistentDataPath + "/" + SessionData.myFileName + ".vt";

        //Ariel
        FileStream stream;
        if (File.Exists(path))
        {
            stream = new FileStream(path, FileMode.Open);
        }
        else
        {
            stream = new FileStream(path, FileMode.Create);
        }
        Save data = new Save(current);

        formatter.Serialize(stream, data);
        stream.Close();

    }


    //The following was a unit test designed to debug the save conditions for the playback function.
    public static void SaveFunctionForPlayback(MenuFunctions current, int sceneOrder)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path;
        //Custom File Name
        //Daniel
        if (!File.Exists(Application.persistentDataPath + "/playbackTemp.vt"))
        {
            path = Application.persistentDataPath + "/playbackTemp.vt";
            SessionData.myFileName = "PlaybackTemp";
            Debug.Log("File Exists? " + File.Exists(path));
            while (File.Exists(path))
            {
                path = Application.persistentDataPath + "/playbackTemp" + sceneOrder + ".vt";
                SessionData.myFileName = "playbackTemp" + sceneOrder;
            }
        }

        //Ariel
        path = Application.persistentDataPath + "/playbackTemp.vt";
        Debug.Log(path);
        FileStream stream;
        if (File.Exists(path))
        {
            stream = new FileStream(path, FileMode.Open);
        }
        else
        {
            stream = new FileStream(path, FileMode.Create);
        }
        Save data = new Save(current);

        formatter.Serialize(stream, data);
        stream.Close();

    }

    //The following takes a json file that the user chooses and begins the process of turning the serialized data
    //back into the GameObjects and conditions in the program.
    public static Save loadData()
    {

        string path = Application.persistentDataPath + "/" + SessionData.myFileName + ".vt";//Edited by Daniel
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            Save data = formatter.Deserialize(stream) as Save;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }

}