using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TwoHandedGrabInteractable : XRGrabInteractable
{
    public XRSimpleInteractable secondaryAttachmentPoint;
    public enum RotationMethod {None, Left, Right, Interpolated};
    public RotationMethod rotationMethod;

    private IXRSelectInteractor secondaryInteractor;
    private Quaternion initialRotation;
    // Start is called before the first frame update
    void Start()
    {
        // Listen for interaction with the secondary (left) interaction point of the virtual viewport
        secondaryAttachmentPoint.selectEntered.AddListener(SecondaryGrab);
        secondaryAttachmentPoint.selectExited.AddListener(SecondaryRelease);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        // Check if object is currently being interacted with by two interactors
        if (secondaryInteractor != null && firstInteractorSelecting != null)
        {
            // Change object rotation based on the interactors (controllers) rotation
            firstInteractorSelecting.transform.rotation = CalculateRotation() * Quaternion.Euler(0, 90, 0);

        }

        base.ProcessInteractable(updatePhase);
    }

    protected override void OnSelectEntered(SelectEnterEventArgs interactor)
    {   
        // Store initial rotation before manipulation
        initialRotation = interactor.interactorObject.transform.rotation;

        base.OnSelectEntered(interactor);
    }

    protected override void OnSelectExited(SelectExitEventArgs interactor)
    {
        // Restore initial rotation when interaction stops
        interactor.interactorObject.transform.rotation = initialRotation;
        secondaryInteractor = null;

        base.OnSelectExited(interactor);
    }

    private Quaternion CalculateRotation()
    // Calculate rotation based on chosen method
    {
        Quaternion targetRotation;
        Vector3 primaryPosition = firstInteractorSelecting.transform.localPosition;
        Vector3 primaryUp = firstInteractorSelecting.transform.up;
        Vector3 secondaryPosition = secondaryInteractor.transform.localPosition;
        Vector3 secondaryUp = secondaryInteractor.transform.up;

        if (rotationMethod == RotationMethod.None)
        // No up-vector is used
        {
            targetRotation = Quaternion.LookRotation(secondaryPosition - primaryPosition);
        }
        else if (rotationMethod == RotationMethod.Right)
        // Up-vector based on right controller
        {
            targetRotation = Quaternion.LookRotation(secondaryPosition - primaryPosition, primaryUp);
        }
        else if (rotationMethod == RotationMethod.Left)
        // Up-vector based on left controller
        {
            targetRotation = Quaternion.LookRotation(secondaryPosition - primaryPosition, secondaryUp);
        }
        else
        // Up-vector based on the average of both controllers
        {
            // Interpolate between both up-vectors
            Vector3 averageUpVector = Vector3.Lerp(primaryUp, secondaryUp, 0.5f);
            targetRotation = Quaternion.LookRotation(secondaryPosition - primaryPosition, averageUpVector);
        }

        return targetRotation;

    }

    public void SecondaryGrab(SelectEnterEventArgs interactor)
    {
        Debug.Log("Secondary Grab");
        secondaryInteractor = interactor.interactorObject;
    }

    public void SecondaryRelease(SelectExitEventArgs interactor)
    {
        Debug.Log("Secondary Release");
        secondaryInteractor = null;
    }
}
