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
    public AnimationRecorder animationRecorder;
    public ViewportManager viewportManager;
    public AnimationManager animationManager;
    public TeleportationController teleportationController;
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
    public enum InteractionMode{exploration, recording, visualizing};
    public InteractionMode interactionMode;

    private InputDevice device;
    private bool wasActivated = false;
    private bool wasSelected = false;
    private Vector2 inputAxis;
    private List<Image> radialMenuSprites = new List<Image>();
    private Image selectedSprite;
    private Component selectedScript;

    // Start is called before the first frame update
    void Start()
    {
        radialMenuCanvas.gameObject.SetActive(false); // Disable UI for mode selection on startup

        interactionMode = InteractionMode.exploration;

        foreach (var item in radialMenuItems)
        // Assign each UI element to List
        {
            Image image = item.GetComponent<Image>();
            radialMenuSprites.Add(image);
        }
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
    {
        InputHelpers.IsPressed(device, selectionButton, out bool isSelected);

        if (radialMenuCanvas.gameObject.activeSelf)
        {
            if (isSelected && !wasSelected)
            {
                wasSelected = true;
                ActivateScript(interactionMode);
                ToggleVisibility();
                return true;
            }

            else
            {
                wasSelected = false;
                return false;
            }
        }

        else
        {
            return false;
        }
    }

    void ToggleVisibility()
    // Toggle the visibility of the radial menu UI element
    {
        radialMenuCanvas.gameObject.SetActive(!radialMenuCanvas.gameObject.activeSelf);

        if (radialMenuCanvas.gameObject.activeSelf)
        {
            gameObject.GetComponent<DeviceBasedSnapTurnProvider>().enabled = false;
        }
        else
        {
            gameObject.GetComponent<DeviceBasedSnapTurnProvider>().enabled = true;
        }
    }

    void InputSeperation(Vector2 inputVector)
    // Detects the input of the analog stick and divides it into 4 quadrants
    // Each quadrant corresponds to a different sprite and the changes the interactionMode accordingly
    {
        if
        (
            inputVector.x > lowerLimit &&   // Detect left limit
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
            inputVector.y > lowerLimit && 
            inputVector.y < upperLimit && 
            inputVector.x < -activationThreshold
        )
        {
            //Debug.Log("Left");
            ResetSpriteColor();
            selectedSprite = radialMenuSprites[1];
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
            selectedSprite = radialMenuSprites[2];
            interactionMode = InteractionMode.exploration;
        }
        else
        {
            ResetSpriteColor();
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

    public void ActivateScript(InteractionMode mode)
    // Activates a script depending on the current value of the interactionMode variable
    {
        DeactivateScripts();
        if (mode == InteractionMode.exploration)
        {
            teleportationController.EnableScript();
        }

        if (mode == InteractionMode.recording)
        {
            animationRecorder.EnableScript();
            viewportManager.EnableScript();
        }

        if (mode == InteractionMode.visualizing)
        {
            animationManager.EnableScript();
        }
    }

    void DeactivateScripts()
    {
        animationRecorder.DisableScript();
        viewportManager.DisableScript();
        animationManager.DisableScript();
        teleportationController.DisableScript();
    }
}
