using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


//Camera Manager Module.
//Organized and tested by Daniel Thomas
public class CameraMgr : MonoBehaviour
{
    public static CameraMgr inst;
    private void Awake()
    {
        inst = this;
    }

    public Button topDown;
    public Button standCamera;

    //Start().
    //Written by Daniel Thomas and Catherine Stettler.
    // Start is called before the first frame update. Adds listeners to the appropriate UI elements
    //and sets the initial position of the camera.
    void Start()
    {
        topDown.onClick.AddListener(topDownCamera);
        standCamera.onClick.AddListener(stdCamera);
        setCameraDefault();
    }
    public GameObject RTSCameraRig;

    public GameObject YawNode; // Child of RTSCameraRig
    public GameObject PitchNode; // Child of YawNode
    public GameObject RollNode; // Child of PitchNode
                                // Camera is child of RollNode

    public float cameraMoveSpeed = 100f;
    public float cameraTurnRate = 50f;

    public Vector3 currentYawEulerAngles = Vector3.zero;
    public Vector3 currentPitchEulerAngles = Vector3.zero;

    private Vector3 defaultCameraSettings;
    private Vector3 defaultCameraPitch;


    //setCameraDefault().
    //Written by Catherine Stettler.
    //Sets the default camera settings in order to revert back from top-down camera view.
    private void setCameraDefault()
    {
        defaultCameraSettings = YawNode.transform.localPosition;
        defaultCameraPitch = PitchNode.transform.localEulerAngles;
    }

    //Update.
    //Original code written by Daniel Thomas.
    //Updates the state of the program at the start of each frame. Also handles movement and manipulation of the
    //camera.
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            YawNode.transform.Translate(Vector3.forward * Time.deltaTime * cameraMoveSpeed);
        }
        if (Input.GetKey(KeyCode.S))
        {
            YawNode.transform.Translate(Vector3.back * Time.deltaTime * cameraMoveSpeed);
        }
        if (Input.GetKey(KeyCode.D))
        {
            YawNode.transform.Translate(Vector3.right * Time.deltaTime * cameraMoveSpeed);
        }
        if (Input.GetKey(KeyCode.A))
        {
            YawNode.transform.Translate(Vector3.left * Time.deltaTime * cameraMoveSpeed);
        }
        if (Input.GetKey(KeyCode.R))
        {
            YawNode.transform.Translate(Vector3.up * Time.deltaTime * cameraMoveSpeed);
        }
        if (Input.GetKey(KeyCode.F) && YawNode.transform.position.y > 1)
        {
            YawNode.transform.Translate(Vector3.down * Time.deltaTime * cameraMoveSpeed);
        }

        currentYawEulerAngles = YawNode.transform.localEulerAngles;
        if (Input.GetKey(KeyCode.Q))
        {
            currentYawEulerAngles.y -= cameraTurnRate * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.E))
        {
            currentYawEulerAngles.y += cameraTurnRate * Time.deltaTime;
        }
        YawNode.transform.localEulerAngles = currentYawEulerAngles;

        currentPitchEulerAngles = PitchNode.transform.localEulerAngles;
        if (Input.GetKey(KeyCode.Z))
        {
            currentPitchEulerAngles.x -= cameraTurnRate * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.X))
        {
            currentPitchEulerAngles.x += cameraTurnRate * Time.deltaTime;
        }
        PitchNode.transform.localEulerAngles = currentPitchEulerAngles;

        //Original code for future use of a top down camera
        /*if (Input.GetKeyDown(KeyCode.C))
        {
            if (isRTSMode)
            {
                RTSCameraLastPosition = YawNode.transform.localPosition;
                RTSCameraLastRotation = YawNode.transform.localEulerAngles;
                YawNode.transform.SetParent(SelectionMgr.inst.selectedEntity.cameraRig.transform);
                YawNode.transform.localPosition = Vector3.zero;
                YawNode.transform.localEulerAngles = Vector3.zero;
            }
            else
            {

                YawNode.transform.SetParent(RTSCameraRig.transform);
                YawNode.transform.localPosition = RTSCameraLastPosition;
                YawNode.transform.localEulerAngles = RTSCameraLastRotation;
            }
            isRTSMode = !isRTSMode;

        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
    public bool isRTSMode = true;
    public Vector3 RTSCameraLastPosition;
    public Vector3 RTSCameraLastRotation;*/
    }

    //topDownCamera().
    //Written by Catherine Stettler.
    //Changes the angle of the camera to a top-down view of the stage.
    public void topDownCamera()
    {
        Vector3 tempPitch = Vector3.zero;
        Vector3 tempYaw = Vector3.zero;
        
        tempPitch = PitchNode.transform.localEulerAngles;

        tempPitch.x = 90;
        tempYaw.y = 30;

        YawNode.transform.localPosition = tempYaw;
        PitchNode.transform.localEulerAngles = tempPitch;
    }

    //stdCamera().
    //Written by Catherine Stettler.
    //Resets the camera to its initial position.
    public void stdCamera()
    {
        YawNode.transform.localPosition = defaultCameraSettings;
        PitchNode.transform.localEulerAngles = defaultCameraPitch;
    }

}
