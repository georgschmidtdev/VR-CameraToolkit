using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HandPresence : MonoBehaviour
{
    private InputDevice leftController;
    private InputDevice rightController;

    private Animator handAnimator;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!leftController.isValid)
        {
            InitializeControllers(0);
        }
        
        if (!rightController.isValid)
        {
            InitializeControllers(1);
        }

        ControllerInputListener();
    }

    void InitializeControllers(int controllerId)
    {
        if (controllerId == 0)
        {
            List<InputDevice> leftControllers = new List<InputDevice>();
            InputDeviceCharacteristics leftControllerCharacteristics = InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.Left;
            InputDevices.GetDevicesWithCharacteristics(leftControllerCharacteristics, leftControllers);

            if (leftControllers.Count != 0)
            {
                leftController = leftControllers[0];
                Debug.Log(leftController.name + leftController.characteristics);
            }
        }

        if (controllerId == 1)
        {
            List<InputDevice> rightControllers = new List<InputDevice>();
            InputDeviceCharacteristics rightControllerCharacteristics = InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.Right;
            InputDevices.GetDevicesWithCharacteristics(rightControllerCharacteristics, rightControllers);

            if (rightControllers.Count != 0)
            {
                rightController = rightControllers[0];
                Debug.Log(rightController.name + rightController.characteristics);
            }
        }
    }

    void ControllerInputListener()
    {
        if (rightController.TryGetFeatureValue(CommonUsages.trigger, out float rightTriggerValue))
        {
            UpdateHandAnimation(rightTriggerValue);
        }

        if (rightController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 rightPrimary2DAxisValues) && rightPrimary2DAxisValues != Vector2.zero)
        {
            Debug.Log("Right Analog stick at " + rightPrimary2DAxisValues);
        }

        if (rightController.TryGetFeatureValue(CommonUsages.gripButton, out bool rightGripButtonValue) && rightGripButtonValue)
        {
            Debug.Log("Right grip button pressed");
        }
    }

    void UpdateHandAnimation(float value)
    {
        
    }
}
