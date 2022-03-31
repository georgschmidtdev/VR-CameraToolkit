using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LookAtPlayer : MonoBehaviour
{
    public Canvas informationCanvas;

    private GameObject vrCamera;
    private static bool scriptIsEnabled = false;
    private TextMeshProUGUI animationNameField;
    private TextMeshProUGUI animationLengthField;
    private TextMeshProUGUI animationFramerateField;
    private GameObject informationPanel;

    // Start is called before the first frame update
    void Start()
    {
        DisableScript();
        vrCamera = GameObject.FindWithTag("VRCamera");
    }

    // Update is called once per frame
    void Update()
    {
        if (scriptIsEnabled)
        {
            RotateCanvas();
        }
    }

    void RotateCanvas()
    {
        Quaternion newRotation;
        Vector3 direction = vrCamera.transform.position - informationCanvas.gameObject.transform.position;
        newRotation = Quaternion.LookRotation(-direction, Vector3.up);
        informationCanvas.gameObject.transform.rotation = newRotation;
    }

    public void DisableScript()
    {
        scriptIsEnabled = false;
        informationCanvas.gameObject.SetActive(false);
    }

    public void EnableScript()
    {
        scriptIsEnabled = true;
        informationCanvas.gameObject.SetActive(true);
    }
}
