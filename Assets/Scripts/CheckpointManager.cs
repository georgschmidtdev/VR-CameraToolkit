using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Input;
using UnityEngine;
using UnityEngine.XR;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit;

public class CheckpointManager : MonoBehaviour
{
    public XROrigin rig;
    public XRRayInteractor checkpointRay;
    public XRNode inputSource;
    public InputHelpers.Button activationButton;
    public float activationThreshold = 0.2f;
    public GameObject instanceParent;
    public GameObject instancePrefab;
    public LayerMask checkpointRayMask;

    private bool scriptIsEnabled = false;
    private bool wasPressed = false;
    private List<GameObject> instancedCheckpoints;
    private Vector3 currentTargetPosition;
    private GameObject currentInstance;

    // Start is called before the first frame update
    void Start()
    {
        instancedCheckpoints = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(inputSource);

        
        if (scriptIsEnabled)
        {
            UpdateRayCastHit(currentTargetPosition, checkpointRayMask);

            if (CheckIfActive(device))
            {
                SetNewCheckpoint();
            }
        }
    }
    bool CheckIfActive(InputDevice device)
    {
        InputHelpers.IsPressed(device, activationButton, out bool isPressed);
        
        if (isPressed && !wasPressed)
        {
            wasPressed = true;
            return true;
        }
        else if (!isPressed && wasPressed)
        {
            wasPressed = false;
            return false;
        }
        else
        {
            return false;
        }
    }

    void UpdateRayCastHit(Vector3 outputPosition, LayerMask mask)
    {
        checkpointRay.raycastMask = mask;
        checkpointRay.TryGetHitInfo(out outputPosition, out Vector3 normal, out int positionInLine, out bool isValidTarget);
    }

    void SetNewCheckpoint()
    {
        if (checkpointRay.TryGetHitInfo(out Vector3 hitPosition, out Vector3 normal, out int positionInLine, out bool isValidTarget))
        {
            instancedCheckpoints.Add(Instantiate(instancePrefab, hitPosition, Quaternion.Euler(0f, 0f, 0f), instanceParent.transform));
            currentInstance = instancedCheckpoints[instancedCheckpoints.Count - 1];
        }
    }

    public void DisableScript()
    {
        scriptIsEnabled = false;
    }

    public void EnableScript()
    {
        scriptIsEnabled = true;
    }
}
