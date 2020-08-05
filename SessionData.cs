//Daniel
//This class holds global data of the currently loaded file.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SessionData
{
    public static string myFileName = null;
    public static string selectedStageType = null;

    public static string FileName
    {
        get
        {
            return myFileName;
        }
        set
        {
            myFileName = value;
        }
    }

    public static string StageType
    {
        get
        {
            return selectedStageType;
        }
        set
        {
            selectedStageType = value;
        }
    }

}
