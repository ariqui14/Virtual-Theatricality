//Daniel
//This script dynamically sizes the selectionCylinder to the size of the object's mesh renderer.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionCircle : MonoBehaviour
{
    GameObject myObjectWithMesh;
    GameObject selectionCircle;
    
    //This start function sets the initial position of the selectionCircle and the scale of the selectionCircle based on the parent Object
    void Start()
    {
        try
        {
            selectionCircle = gameObject;
            selectionCircle.transform.localPosition = new Vector3(0, 0.01f, 0);
            selectionCircle.transform.localEulerAngles = selectionCircle.transform.parent.transform.localEulerAngles;
            Bounds myBounds = RecursiveMeshBB(selectionCircle.transform.parent.gameObject);
            float newSize = FindMax(myBounds);
            selectionCircle.transform.localScale = new Vector3(newSize / selectionCircle.transform.parent.localScale.x, 0.001f, newSize / selectionCircle.transform.parent.localScale.x);


        }
        catch
        {
            Debug.Log("Can't display selection circle");
        }
       
    }

    //This function finds the largest of the 3 bounds of the parent gameObject and returns it.
    float FindMax(Bounds bounds)
    {
        float output = bounds.size.x;
        if (bounds.size.y > output)
            output = bounds.size.y;
        if (bounds.size.z > output)
            output = bounds.size.z;
        return output * 1.5f;
    }

    //This function recursively checks all children of the parent object to find the largest MeshRenderer.
    static public Bounds RecursiveMeshBB(GameObject go)
    {
        List<MeshRenderer> mfs = new List<MeshRenderer>();
        mfs.AddRange(go.GetComponentsInChildren<MeshRenderer>());

        if (mfs.Count > 0)
        {
            Bounds b = mfs[0].bounds;
            for (int i = 1; i < mfs.Count; i++)
            {
                b.Encapsulate(mfs[i].bounds);
            }
            return b;
        }
        else
            return new Bounds();
    }

}
