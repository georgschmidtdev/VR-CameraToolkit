using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TwoHandedGrabInteractable : XRGrabInteractable
{
    public XRSimpleInteractable secondaryAttachmentPoint;

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
            this.trackPosition = false;
            this.trackRotation = false;

            // Change object rotation based on the interactors (controllers) rotation
            //firstInteractorSelecting.transform.rotation = CalculateRotation() * Quaternion.Euler(0f, 90f, 0f);
            gameObject.transform.rotation = CalculateRotation()* Quaternion.Euler(90f, 90f, 0f);
            gameObject.transform.position = CalculatePosition();
        }

        else
        {
            this.trackPosition = true;
            this.trackRotation = true;
        }

        base.ProcessInteractable(updatePhase);
    }

    protected override void OnSelectEntered(SelectEnterEventArgs interactor)
    {   
        // Store initial rotation before manipulation
        initialRotation = interactor.interactorObject.transform.localRotation;

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
        Vector3 primaryPosition = firstInteractorSelecting.transform.position;
        Vector3 primaryUp = firstInteractorSelecting.transform.up;
        Vector3 secondaryPosition = secondaryInteractor.transform.position;
        Vector3 secondaryUp = secondaryInteractor.transform.up;

        // Up-vector based on the average of both controllers
        
        // Interpolate between both up-vectors
        Vector3 averageUpVector = Vector3.Lerp(primaryUp, secondaryUp, 0.5f);
        targetRotation = Quaternion.LookRotation(secondaryPosition - primaryPosition, averageUpVector);

        return targetRotation;
    }

    private Vector3 CalculatePosition()
    {
        Vector3 targetPosition;
        Vector3 primaryPosition = firstInteractorSelecting.transform.position;
        Vector3 secondaryPosition = secondaryInteractor.transform.position;

        targetPosition = Vector3.Lerp(primaryPosition, secondaryPosition, 0.5f);

        return targetPosition;
    }

    public void SecondaryGrab(SelectEnterEventArgs interactor)
    {
        secondaryInteractor = interactor.interactorObject;
    }

    public void SecondaryRelease(SelectExitEventArgs interactor)
    {
        secondaryInteractor = null;
    }
}
