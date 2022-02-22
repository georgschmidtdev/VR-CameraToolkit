using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HandPresence : MonoBehaviour
{
    public InputDeviceCharacteristics controllerCharacteristics;
    public GameObject instancePrefab;
    
    private List<InputDevice> controllers = new List<InputDevice>();
    private GameObject instancedModel;
    private Animator handAnimator;

    // Start is called before the first frame update
    void Start()
    {
        InitializeControllers();
    }

    // Update is called once per frame
    void Update()
    {
        if (controllers.Count == 0)
        {
            InitializeControllers();
        }

        UpdateHandAnimation();
    }

    void InitializeControllers()
    {
        List<InputDevice> detectedControllers = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, detectedControllers);

        foreach (var item in detectedControllers)
        {
            if (!controllers.Contains(item))
            {
                controllers.Add(item);
                Debug.Log(item.name);
            }

            instancedModel = Instantiate(instancePrefab, transform);
            handAnimator = instancedModel.GetComponent<Animator>();
        }
    }

    void UpdateHandAnimation()
    {
        foreach (var item in controllers)
        {
            if (item.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
            {
                handAnimator.SetFloat("trigger", triggerValue);
            }

            if (item.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
            {
                handAnimator.SetFloat("grip", gripValue);
            }
        }
    }
}
