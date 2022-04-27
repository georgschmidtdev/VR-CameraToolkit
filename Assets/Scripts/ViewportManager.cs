using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;

public class ViewportManager : MonoBehaviour
{
    public XRNode leftInputDevice;
    public XRNode rightInputDevice;
    public InputHelpers.Button activationButton;
    public GameObject virtualViewport;
    public GameObject cameraSettings;
    public Camera viewportCamera;
    public Canvas indicatorCanvas;
    public TextMeshProUGUI framerateIndicator;
    public TextMeshProUGUI focalLengthIndicator;
    public TextMeshProUGUI aspectRatioIndicator;
    public RectTransform viewport;
    public RectTransform indicators;
    public Image recordingIndicator;
    public TMP_Dropdown framerateDropdown;
    public TMP_Dropdown aspectRatioDropdown;
    public TMP_Dropdown focalLengthDropdown;

    private bool scriptIsEnabled = false;
    private bool wasActivated = false;
    private InputDevice leftDevice;
    private InputDevice rightDevice;
    private GameObject vrCamera;
    private AnimationRecorder animationRecorder;
    private static Color defaultColor = Color.white;
    private static Color recordingColor = Color.red;
    private int[] framerates = new int[]{24, 25, 30, 50, 60, 120};
    private List<Vector2> aspectRatios;
    private List<float> focalLengths;
    private Vector2 standardRatio = new Vector2(4.0f, 3.0f);
    private Vector2 squareRatio = new Vector2(1.0f, 1.0f);
    private Vector2 hdtvRatio = new Vector2(16.0f, 9.0f);
    private Vector2 still35Ratio = new Vector2(3.0f, 2.0f);
    private Vector2 widescreenRatio = new Vector2(14.0f, 9.0f);
    private Vector2 film35Ratio = new Vector2(1.85f, 1.0f);
    private Vector2 panavisionRatio = new Vector2(2.39f, 1.0f);
    private float wideFocal = 25.0f;
    private float classicFocal = 36.0f;
    private float normalFocal = 50.0f;
    private float portraitFocal = 80.0f;
    private float teleFocal = 135.0f;
    private Vector2 inputAxis;
    private bool changingFocalLength = false;

    // Start is called before the first frame update
    void Start()
    {
        vrCamera = GameObject.FindWithTag("MainCamera");
        animationRecorder = GetComponent<AnimationRecorder>();
        leftDevice = InputDevices.GetDeviceAtXRNode(leftInputDevice);
        rightDevice = InputDevices.GetDeviceAtXRNode(rightInputDevice);

        VariableSetup();
        UpdateCameraSettings();

        DisableScript();
    }

    // Update is called once per frame
    void Update()
    {
        if (scriptIsEnabled)
        {
            CheckIfActive();
            CalculateFocalLength();
            UpdateCameraSettings();
        }
    }

    void CalculateFocalLength()
    {
        float activationThreshhold = 0.5f;
        Vector2 normalizedInput;
        Vector2 initialVector;
        rightDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputAxis);
        normalizedInput = inputAxis.normalized;
        
        if (inputAxis.magnitude > activationThreshhold && !changingFocalLength)
        {
            changingFocalLength = true;
            initialVector = inputAxis;
        }

        else if (inputAxis.magnitude < activationThreshhold && changingFocalLength)
        {
            changingFocalLength = false;
        }

        else if (inputAxis.magnitude > activationThreshhold && changingFocalLength)
        {
        }
    }

    void CheckIfActive()
    // Unitys integrated GetKeyDown() function is not available for XR-Controller based inputs
    // This function emulates the same behaviour for a given (InputDevice device) and button (activationButton)
    {
        InputHelpers.IsPressed(leftDevice, activationButton, out bool isPressed);
        
        if (isPressed && !wasActivated)
        {
            wasActivated = true;
            ToggleVisibility();
        }
        
        else if (!isPressed && wasActivated)
        {
            wasActivated = false;
        }
    }

    void VariableSetup()
    {
        aspectRatios = new List<Vector2>();
        focalLengths = new List<float>();

        aspectRatios.Add(standardRatio);
        aspectRatios.Add(squareRatio);
        aspectRatios.Add(hdtvRatio);
        aspectRatios.Add(still35Ratio);
        aspectRatios.Add(widescreenRatio);
        aspectRatios.Add(film35Ratio);
        aspectRatios.Add(panavisionRatio);

        focalLengths.Add(wideFocal);
        focalLengths.Add(classicFocal);
        focalLengths.Add(normalFocal);
        focalLengths.Add(portraitFocal);
        focalLengths.Add(teleFocal);

        PopulateDropdownMenus();
    }

    void PopulateDropdownMenus()
    {
        foreach (var item in framerates)
        {
            TMP_Dropdown.OptionData newOption = new TMP_Dropdown.OptionData();
            newOption.text = item.ToString();
            framerateDropdown.options.Add(newOption);
        }

        foreach (var item in aspectRatios)
        {
            TMP_Dropdown.OptionData newOption = new TMP_Dropdown.OptionData();
            newOption.text = item.ToString();
            aspectRatioDropdown.options.Add(newOption);
        }

        foreach (var item in focalLengths)
        {
            TMP_Dropdown.OptionData newOption = new TMP_Dropdown.OptionData();
            newOption.text = item.ToString();
            focalLengthDropdown.options.Add(newOption);
        }
    }

    public void UpdateCameraSettings()
    {
        UpdateViewport();
        UpdateInterface();
        animationRecorder.SetFramerate(framerates[framerateDropdown.value]);
    }

    void UpdateViewport()
    // Apply changes
    {
        float currentAspectRatio = aspectRatios[aspectRatioDropdown.value].x / aspectRatios[aspectRatioDropdown.value].y;

        float width = 0.5f;
        float height = Mathf.Lerp(0.0f, width, Mathf.InverseLerp(0.0f, aspectRatios[aspectRatioDropdown.value].x, aspectRatios[aspectRatioDropdown.value].y)); // Calculate viewport height based on the given aspect ratio in relation to the maximum of the width
        Vector2 newViewportSize = new Vector2(width, height); // Create new Vector2 to resize the viewport panel
        Vector2 newIndicatorSize = new Vector2(newViewportSize.x, newViewportSize.y + 0.06f); // Copy size of viewport for interface with offset for text elements

        viewport.sizeDelta = newViewportSize;
        indicators.sizeDelta = newIndicatorSize;

        viewportCamera.aspect = currentAspectRatio;
        viewportCamera.focalLength = focalLengths[focalLengthDropdown.value];
    }

    void UpdateInterface()
    {
        framerateIndicator.text = framerates[framerateDropdown.value].ToString() + " fps";
        focalLengthIndicator.text = focalLengths[focalLengthDropdown.value].ToString() + " mm";
        aspectRatioIndicator.text = aspectRatios[aspectRatioDropdown.value].x.ToString() + ":" + aspectRatios[aspectRatioDropdown.value].y.ToString();
    }

    public void StartRecordingIndicator()
    {   
        recordingIndicator.color = recordingColor;
    }

    public void StopRecordingIndicator()
    {
        recordingIndicator.color = defaultColor;
    }

    public void ResetViewportPosition()
    {
        Vector3 defaultPosition = vrCamera.transform.position + new Vector3(0f, -0.2f, 0.5f);
        virtualViewport.gameObject.transform.position = defaultPosition;
    }

    void ToggleVisibility()
    {
        cameraSettings.gameObject.SetActive(!cameraSettings.activeSelf);
    }

    public void DisableScript()
    {
        virtualViewport.gameObject.SetActive(false);
        cameraSettings.gameObject.SetActive(false);
        scriptIsEnabled = false;
    }

    public void EnableScript()
    {
        virtualViewport.gameObject.SetActive(true);
        scriptIsEnabled = true;
    }
}