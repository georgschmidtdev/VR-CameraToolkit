using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class ModeManager : MonoBehaviour
// Manages the different interaction modes based on the users UI selection
{
    public GameObject rig;
    public XRNode inputDevice;
    public InputHelpers.Button activationButton;
    public InputHelpers.Button selectionButton;
    public List<InputHelpers.Button> buttons;
    public float activationThreshold = 0.2f;
    public float lowerLimit = -0.5f;
    public float upperLimit = 0.5f;
    public Canvas radialMenuCanvas;
    public List<GameObject> radialMenuItems;
    public Color defaultColor = Color.white;
    public Color highlightedColor = Color.blue;
    public enum InteractionMode{none, recording, planning, visualizing, exporting};
    public InteractionMode interactionMode;

    private InputDevice device;
    private bool wasActivated = false;
    private bool wasSelected = false;
    private AnimationRecorder animationRecorder;
    private CheckpointManager checkpointManager;
    private AnimationManager animationManager;
    private Vector2 inputAxis;
    private List<Image> radialMenuSprites = new List<Image>();
    private Image selectedSprite;
    private Image middleSprite;
    private Component selectedScript;

    // Start is called before the first frame update
    void Start()
    {
        animationRecorder = GetComponent<AnimationRecorder>();
        checkpointManager = GetComponent<CheckpointManager>();
        animationManager = GetComponent<AnimationManager>();
        //DeactivateScripts(); // Make sure all function scripts are disabled by default

        radialMenuCanvas.gameObject.SetActive(false); // Disable UI for mode selection on startup

        foreach (var item in radialMenuItems)
        // Assign each UI element to List
        {
            Image image = item.GetComponent<Image>();
            radialMenuSprites.Add(image);
        }

        middleSprite = radialMenuSprites[radialMenuSprites.Count - 1];
    }

    // Update is called once per frame
    void Update()
    {
        if (!device.isValid)
        // Check if input device has been assigned yet
        {
            device = InputDevices.GetDeviceAtXRNode(inputDevice);
        }
        
        device.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputAxis);

        CheckIfActive(device);
        CheckIfSelected(device);
        InputSeperation(inputAxis);

        if (selectedSprite != null)
        // Check if a sprite has been selected
        {
            ChangeSpriteColor(selectedSprite);
        }
    }

    bool CheckIfActive(InputDevice device)
    // Unitys integrated GetKeyDown() function is not available for XR-Controller based inputs
    // This function emulates the same behaviour for a given (InputDevice device) and button (activationButton)
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
    // See CheckIfActive()
    {
        InputHelpers.IsPressed(device, selectionButton, out bool isSelected);
        
        if (isSelected && !wasSelected)
        {
            wasSelected = true;
            ActivateScript(interactionMode);
            ShowCurrentModeSprite();
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
    // Toggle the visibility of the radial menu UI element
    {
        bool status = radialMenuCanvas.gameObject.activeSelf;
        radialMenuCanvas.gameObject.SetActive(!status);
    }

    void InputSeperation(Vector2 inputVector)
    // Detects the input of the analog stick and divides it into 4 quadrants
    // Each quadrant corresponds to a different sprite and the changes the interactionMode accordingly
    {
        if
        (
            inputVector.x > lowerLimit &&   // Detext left limit
            inputVector.x < upperLimit &&   // Detect right limit
            inputVector.y > activationThreshold // Detect minimum value
        )
        {
            //Debug.Log("Up");
            ResetSpriteColor(); // Reset sprite color to the default before updating
            selectedSprite = radialMenuSprites[0];  // Store currently selected sprite
            interactionMode = InteractionMode.recording;    // Change interaction mode
        }

        else if
        (
            inputVector.x > lowerLimit && 
            inputVector.x < upperLimit && 
            inputVector.y < -activationThreshold
        )
        {
            //Debug.Log("Down");
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
            //Debug.Log("Left");
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
            //Debug.Log("Right");
            ResetSpriteColor();
            selectedSprite = radialMenuSprites[3];
            interactionMode = InteractionMode.exporting;
        }

        else
        // Default position
        // True when both x and y values of the inputAxis do not exceed the minimum value activationThreshold
        {
            ResetSpriteColor();
            selectedSprite = radialMenuSprites[radialMenuSprites.Count-1]; // Store center sprite as selection
            interactionMode = InteractionMode.none; // Select default interaction mode
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

    void ShowCurrentModeSprite()
    {
        middleSprite.sprite = selectedSprite.sprite;
    }

    void ActivateScript(InteractionMode mode)
    // Activates a script depending on the current value of the interactionMode variable
    // Calls the corresponding scripts EnableScript() function
    {
        DeactivateScripts();
        if (mode == InteractionMode.none)
        {
            Debug.Log("Default mode selected");
        }

        if (mode == InteractionMode.recording)
        {
            animationRecorder.EnableScript();
            Debug.Log("Recording");
        }

        if (mode == InteractionMode.planning)
        {
            checkpointManager.EnableScript();
            Debug.Log("Planning");
        }

        if (mode == InteractionMode.exporting)
        {
            Debug.Log("TODO: Export functionality");
        }

        if (mode == InteractionMode.visualizing)
        {
            animationManager.EnableScript();
            Debug.Log("Visualizing");
        }
    }

    void DeactivateScripts()
    {
        animationRecorder.DisableScript();
        checkpointManager.DisableScript();
        animationManager.DisableScript();
    }
}
