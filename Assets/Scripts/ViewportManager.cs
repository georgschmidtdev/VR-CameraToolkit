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
    public TMP_Dropdown sensorSizeDropdown;
    public TMP_Dropdown aspectRatioDropdown;
    public TMP_Dropdown focalLengthDropdown;

    private bool scriptIsEnabled = false;
    private bool wasActivated = false;
    private GameObject vrCamera;
    private AnimationRecorder animationRecorder;
    private AspectRatioManager aspectRatioManager;
    private static Color defaultColor = Color.white;
    private static Color recordingColor = Color.red;
    private float[] framerates = new float[]{24f, 25f, 30f, 50f, 60f, 120f};
    private Dictionary<string, Vector2> sensorDictionary;
    private Vector2 super8 = new Vector2(5.79f, 4.01f);
    private Vector2 super16 = new Vector2(12.52f, 7.41f);
    private Vector2 academy35 = new Vector2(21.0f, 15.2f);
    private Vector2 super35 = new Vector2(24.89f, 18.66f);
    private Vector2 alexa65 = new Vector2(54.12f, 25.59f);
    private Vector2 imax70 = new Vector2(70.41f, 52.63f);
    private List<Vector2> aspectRatios;
    private Vector2 standardRatio = new Vector2(4.0f, 3.0f);
    private Vector2 squareRatio = new Vector2(1.0f, 1.0f);
    private Vector2 hdtvRatio = new Vector2(16.0f, 9.0f);
    private Vector2 still35Ratio = new Vector2(3.0f, 2.0f);
    private Vector2 widescreenRatio = new Vector2(14.0f, 9.0f);
    private Vector2 film35Ratio = new Vector2(1.85f, 1.0f);
    private Vector2 panavisionRatio = new Vector2(2.39f, 1.0f);
    private List<float> focalLengths;
    private float wideFocal = 25.0f;
    private float classicFocal = 36.0f;
    private float normalFocal = 50.0f;
    private float portraitFocal = 80.0f;
    private float teleFocal = 135.0f;
    private Vector2 inputAxis;

    // Start is called before the first frame update
    void Start()
    {
        vrCamera = GameObject.FindWithTag("MainCamera");
        animationRecorder = GetComponent<AnimationRecorder>();
        aspectRatioManager = viewportCamera.GetComponent<AspectRatioManager>();

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
            //UpdateCameraSettings();
        }
    }

    void CheckIfActive()
    // Unitys integrated GetKeyDown() function is not available for XR-Controller based inputs
    // This function emulates the same behaviour for a given (InputDevice device) and button (activationButton)
    {
        InputDevice leftDevice = InputDevices.GetDeviceAtXRNode(leftInputDevice);
        InputHelpers.IsPressed(leftDevice, activationButton, out bool isPressed);
        
        if (isPressed && !wasActivated)
        {
            wasActivated = true;
            ToggleVisibility();
        }
        
        if (!isPressed && wasActivated)
        {
            wasActivated = false;
        }
    }

    void VariableSetup()
    {
        sensorDictionary = new Dictionary<string, Vector2>();
        aspectRatios = new List<Vector2>();
        focalLengths = new List<float>();

        sensorDictionary.Add("Super8", super8);
        sensorDictionary.Add("Super16", super16);
        sensorDictionary.Add("Academy35", academy35);
        sensorDictionary.Add("Super35", super35);
        sensorDictionary.Add("Alexa65", alexa65);
        sensorDictionary.Add("IMAX70", imax70);
        
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

        foreach (var item in sensorDictionary)
        {
            TMP_Dropdown.OptionData newOption = new TMP_Dropdown.OptionData();
            newOption.text = item.Key;
            sensorSizeDropdown.options.Add(newOption);
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
        viewportCamera.sensorSize = sensorDictionary.GetValueOrDefault(sensorSizeDropdown.options[sensorSizeDropdown.value].text);
        aspectRatioManager.UpdateAspectRatio(new Vector2(aspectRatios[aspectRatioDropdown.value].x, aspectRatios[aspectRatioDropdown.value].y));
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
        scriptIsEnabled = false;
        virtualViewport.gameObject.SetActive(false);
        cameraSettings.gameObject.SetActive(false);
    }

    public void EnableScript()
    {
        scriptIsEnabled = true;
        virtualViewport.gameObject.SetActive(true);
    }
}