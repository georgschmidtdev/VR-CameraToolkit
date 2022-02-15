using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TeleportationController : MonoBehaviour
{
    public XRController leftTeleportRay;
    public XRController rightTeleportRay;
    public InputHelpers.Button activationButton;
    
    private float activationThreshold = 0.5f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    // Enable Ray if check comes back as true
    {
        if (leftTeleportRay)
        {
            leftTeleportRay.gameObject.SetActive(CheckIfActive(leftTeleportRay));
        }

        if (rightTeleportRay)
        {
            rightTeleportRay.gameObject.SetActive(CheckIfActive(rightTeleportRay));
        }
    }

    public bool CheckIfActive(XRController controller)
    // Check if the assigned button is pressed
    {
        InputHelpers.IsPressed(controller.inputDevice, activationButton, out bool result, activationThreshold);
        return result;
    }
}