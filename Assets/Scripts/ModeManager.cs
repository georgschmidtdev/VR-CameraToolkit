using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class ModeManager : MonoBehaviour
{
    public GameObject rig;
    public XRNode inputDevice;
    public InputHelpers.Button activationButton;
    public InputHelpers.Button selectionButton;
    public float activationThreshold = 0.2f;
    public float lowerLimit = -0.5f;
    public float upperLimit = 0.5f;
    public Canvas radialMenuCanvas;
    public List<GameObject> radialMenuItems;
    public Color defaultColor = Color.grey;
    public Color highlightedColor = Color.red;
    public enum InteractionMode{none, recording, planning, visualizing, exporting};
    public InteractionMode interactionMode;

    private bool wasActivated = false;
    private bool wasSelected = false;
    private Component recordingScript;
    private Component checkpointScript;
    private Component visualizationScript;
    private Vector2 inputAxis;
    private List<Image> radialMenuSprites = new List<Image>();
    private Image selectedSprite;
    private Component selectedScript;

    // Start is called before the first frame update
    void Start()
    {
        recordingScript = rig.GetComponent("Animation Recorder");
        checkpointScript = rig.GetComponent("Checkpoint Manager");
        visualizationScript = rig.GetComponent("Visualization Manager");
        DeactivateScripts();

        radialMenuCanvas.gameObject.SetActive(false);

        foreach (var item in radialMenuItems)
        {
            Image image = item.GetComponent<Image>();
            radialMenuSprites.Add(image);
        }

        //selectedSprite = radialMenuSprites[radialMenuSprites.Count-1];
    }

    // Update is called once per frame
    void Update()
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(inputDevice);
        device.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputAxis);

        CheckIfActive(device);
        CheckIfSelected(device);
        InputSeperation(inputAxis);

        if (selectedSprite != null)
        {
            ChangeSpriteColor(selectedSprite);
        }
    }

    bool CheckIfActive(InputDevice device)
    {
        InputHelpers.IsPressed(device, activationButton, out bool isPressed);
        
        if (isPressed && !wasActivated)
        {
            wasActivated = true;
            ToggleVisibility();
            return true;
        }
        else if (!isPressed && wasActivated)
        {
            wasActivated = false;
            return false;
        }
        else
        {
            return false;
        }
    }

    bool CheckIfSelected(InputDevice device)
    {
        InputHelpers.IsPressed(device, selectionButton, out bool isSelected);
        
        if (isSelected && !wasSelected)
        {
            wasSelected = true;
            ActivateScript(interactionMode);
            ToggleVisibility();
            return true;
        }
        else if (!isSelected && wasSelected)
        {
            wasSelected = false;
            return false;
        }
        else
        {
            return false;
        }
    }

    void ToggleVisibility()
    {
        bool status = radialMenuCanvas.gameObject.activeSelf;
        radialMenuCanvas.gameObject.SetActive(!status);
    }

    void InputSeperation(Vector2 inputVector)
    {
        if
        (
            inputVector.x > lowerLimit && 
            inputVector.x < upperLimit && 
            inputVector.y > activationThreshold
        ) 
        {
            Debug.Log("Up");
            ResetSpriteColor();
            selectedSprite = radialMenuSprites[0];
            interactionMode = InteractionMode.recording;
        }

        else if
        (
            inputVector.x > lowerLimit && 
            inputVector.x < upperLimit && 
            inputVector.y < -activationThreshold
        )
        {
            Debug.Log("Down");
            ResetSpriteColor();
            selectedSprite = radialMenuSprites[1];
            interactionMode = InteractionMode.planning;
        }

        else if
        (
            inputVector.y > lowerLimit && 
            inputVector.y < upperLimit && 
            inputVector.x < -activationThreshold
        )
        {
            Debug.Log("Left");
            ResetSpriteColor();
            selectedSprite = radialMenuSprites[2];
            interactionMode = InteractionMode.visualizing;
        }

        else if
        (
            inputVector.y > lowerLimit && 
            inputVector.y < upperLimit && 
            inputVector.x > activationThreshold
        )
        {
            Debug.Log("Right");
            ResetSpriteColor();
            selectedSprite = radialMenuSprites[3];
            interactionMode = InteractionMode.exporting;
        }

        else
        {
            ResetSpriteColor();
            selectedSprite = radialMenuSprites[radialMenuSprites.Count-1];
            interactionMode = InteractionMode.none;
        }
    }

    void ChangeSpriteColor(Image sprite)
    {
        sprite.color = highlightedColor;
    }

    void ResetSpriteColor()
    {
        foreach (var item in radialMenuSprites)
        {
            item.color = defaultColor;
        }
    }

    void ActivateScript(InteractionMode mode)
    {
        if (mode == InteractionMode.none)
        {
            Debug.Log("Default mode selected");
        }

        if (mode == InteractionMode.recording)
        {
            recordingScript.gameObject.SetActive(true);
        }

        if (mode == InteractionMode.planning)
        {
            checkpointScript.gameObject.SetActive(true);
        }

        if (mode == InteractionMode.exporting)
        {
            Debug.Log("TODO: Export functionality");
        }

        if (mode == InteractionMode.exporting)
        {
            
        }
    }

    void DeactivateScripts()
    {
        recordingScript.gameObject.SetActive(false);
        checkpointScript.gameObject.SetActive(false);
        visualizationScript.gameObject.SetActive(false);
    }
}
