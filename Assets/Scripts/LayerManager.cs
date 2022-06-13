using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerManager : MonoBehaviour
{
    public Camera vrCamera;
    public Camera viewportCamera;
    public ContinuousMovement continuousMovement;
    private LayerMask vrCameraMask;
    private LayerMask viewportCameraMask;
    private LayerMask previewCameraMask;
    private LayerMask continuousMovementMask;

    // Start is called before the first frame update
    void Start()
    {
        LayerMaskSetup();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LayerMaskSetup()
    {
        vrCameraMask = Physics.AllLayers;
        viewportCameraMask = Physics.AllLayers;
        previewCameraMask = Physics.AllLayers;
        continuousMovementMask = Physics.AllLayers;

        viewportCameraMask &= ~(1 << LayerMask.NameToLayer("Rig"));
        viewportCameraMask &= ~(1 << LayerMask.NameToLayer("UI"));
        viewportCameraMask &= ~(1 << LayerMask.NameToLayer("Visualization"));

        previewCameraMask &= ~(1 << LayerMask.NameToLayer("Rig"));
        previewCameraMask &= ~(1 << LayerMask.NameToLayer("UI"));
        previewCameraMask &= ~(1 << LayerMask.NameToLayer("Visualization"));

        continuousMovementMask &= ~(1 << LayerMask.NameToLayer("Rig"));
        continuousMovementMask &= ~(1 << LayerMask.NameToLayer("UI"));
        continuousMovementMask &= ~(1 << LayerMask.NameToLayer("Visualization"));
        ApplyLayerMasks();
    }

    void ApplyLayerMasks()
    {
        vrCamera.cullingMask = vrCameraMask;
        viewportCamera.cullingMask = previewCameraMask;
        continuousMovement.collisionLayerMask = continuousMovementMask;
    }

    public LayerMask GetLayerMask()
    {
        return previewCameraMask;
    }
}
