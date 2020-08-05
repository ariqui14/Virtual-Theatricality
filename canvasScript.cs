using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

//This script handles the inspector assignments for the bottom toolbar,
//and switches between interfaces.
//Written by Catherine Stettler
public class canvasScript : MonoBehaviour
{
    public GameObject buildUI;
    public GameObject blockUI;

    public Button buildButton;
    public Button blockButton;

    // Start is called before the first frame update
    //Enables listeners to click buttons in the UI.
    //Enables the Build UI by default when the application starts.
    void Start()
    {
        buildButton.onClick.AddListener(enableBuild);
        blockButton.onClick.AddListener(enableBlock);

        buildUI.SetActive(true);
        blockUI.SetActive(false);
    }

        // Update is called once per frame
        void Update()
        {

        }

    //enableBuild().
    //Written by Catherine Stettler.
    //Enables the Build UI and disables the Main UI.
        private void enableBuild()
        {
            buildUI.SetActive(true);
            blockUI.SetActive(false);
        }
    //enableBlock().
    //Written by Catherine Stettler.
    //Disables the Build UI and enables the Main UI.
        private void enableBlock()
        {
            buildUI.SetActive(false);
            blockUI.SetActive(true);
        }
}
