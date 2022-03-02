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
    public float activationThreshold = 0.2f;
    public float lowerLimit = -0.5f;
    public float upperLimit = 0.5f;
    public Canvas radialMenuCanvas;
    public List<GameObject> radialMenuItems;
    public Color defaultColor = Color.grey;
    public Color highlightedColor = Color.red;

    private bool wasPressed = false;
    private Component recordingScript;
    private Component checkpointScript;
    private Component teleportScript;
    private Component movementScript;
    private Vector2 inputAxis;
    private List<Image> radialMenuSprites = new List<Image>();
    private Image selectedSprite;

    // Start is called before the first frame update
    void Start()
    {
        recordingScript = rig.GetComponent("Animation Recorder");
        checkpointScript = rig.GetComponent("Checkpoint Manager");
        teleportScript = rig.GetComponent("Teleportation Controller");
        movementScript = rig.GetComponent("Continuous Movement");

        radialMenuCanvas.gameObject.SetActive(false);

        foreach (var item in radialMenuItems)
        {
            radialMenuSprites.Add(item.GetComponent<Image>());
        }

        selectedSprite = radialMenuSprites[radialMenuSprites.Count-1];
    }

    // Update is called once per frame
    void Update()
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(inputDevice);
        device.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputAxis);

        CheckIfActive(device);
        InputSeperation(inputAxis);

        if (selectedSprite != null)
        {
            ChangeSpriteColor(selectedSprite);
        }
    }

    bool CheckIfActive(InputDevice device)
    {
        InputHelpers.IsPressed(device, activationButton, out bool isPressed);
        
        if (isPressed && !wasPressed)
        {
            wasPressed = true;
            ToggleVisibility();
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
        }

        else
        {
            ResetSpriteColor();
            selectedSprite = radialMenuSprites[radialMenuSprites.Count-1];
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
}
