using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ViewportManager : MonoBehaviour
{
    public GameObject virtualViewport;
    public Camera viewportCamera;
    public Canvas viewportCanvas;
    public Canvas indicatorCanvas;
    public GameObject recordingIndicator;
    public GameObject framerateIndicator;
    public GameObject focalLengthIndicator;
    public GameObject aspectRatioIndicator;
    public int[] framerates = new int[]{24, 25, 30, 50, 60, 120};
    public enum AspectRatio {Standard, Square, HDTV, Still35, Widescreen, Film35, Panavision};
    public AspectRatio aspectRatio;
    public enum FocalLength {Default, Wide, Classic, Normal, Portrait, Tele};
    public FocalLength focalLength;
    public GameObject framerateDropdown;
    public GameObject aspectRatioDropdown;
    public GameObject focalLengthDropdown;

    private GameObject vrCamera;
    private RectTransform viewport;
    private RectTransform indicators;
    private static Image recordingDot;
    private TextMeshProUGUI framerateText;
    private TextMeshProUGUI focalLengthText;
    private TextMeshProUGUI aspectRatioText;
    private static Color defaultColor = Color.white;
    private static Color recordingColor = Color.red;
    private Vector2 standardRatio = new Vector2(4.0f, 3.0f);
    private Vector2 squareRatio = new Vector2(1.0f, 1.0f);
    private Vector2 hdtvRatio = new Vector2(16.0f, 9.0f);
    private Vector2 still35Ratio = new Vector2(3.0f, 2.0f);
    private Vector2 widescreenRatio = new Vector2(14.0f, 9.0f);
    private Vector2 film35Ratio = new Vector2(1.85f, 1.0f);
    private Vector2 panavisionRatio = new Vector2(2.39f, 1.0f);
    private Dictionary<AspectRatio, Vector2> aspectRatioDictionary;
    private float currentAspectRatio = 0.0f;
    private float defaultFocal = 36.0f;
    private float wideFocal = 25.0f;
    private float classicFocal = 36.0f;
    private float normalFocal = 50.0f;
    private float portraitFocal = 80.0f;
    private float teleFocal = 135.0f;
    private Dictionary<FocalLength, float> focalLengthDictionary;

    // Start is called before the first frame update
    void Start()
    {
        VariableSetup();
        UpdateViewport();
        UpdateInterface();

        vrCamera = GameObject.FindWithTag("MainCamera");
        viewport = viewportCanvas.GetComponent<RectTransform>();
        indicators = indicatorCanvas.GetComponent<RectTransform>();
        recordingDot = recordingIndicator.GetComponent<Image>();
        framerateText = framerateIndicator.GetComponent<TextMeshProUGUI>();
        focalLengthText = focalLengthIndicator.GetComponent<TextMeshProUGUI>();
        aspectRatioText = aspectRatioIndicator.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("h"))
        {
            UpdateViewport();
            UpdateInterface();
        }
    }

    void VariableSetup()
    {
        aspectRatioDictionary = new Dictionary<AspectRatio, Vector2>();
        aspectRatioDictionary.Add(AspectRatio.Standard, standardRatio);
        aspectRatioDictionary.Add(AspectRatio.Square, squareRatio);
        aspectRatioDictionary.Add(AspectRatio.HDTV, hdtvRatio);
        aspectRatioDictionary.Add(AspectRatio.Still35, still35Ratio);
        aspectRatioDictionary.Add(AspectRatio.Widescreen, widescreenRatio);
        aspectRatioDictionary.Add(AspectRatio.Film35, film35Ratio);
        aspectRatioDictionary.Add(AspectRatio.Panavision, panavisionRatio);

        focalLengthDictionary = new Dictionary<FocalLength, float>();
        focalLengthDictionary.Add(FocalLength.Default, defaultFocal);
        focalLengthDictionary.Add(FocalLength.Wide, wideFocal);
        focalLengthDictionary.Add(FocalLength.Classic, classicFocal);
        focalLengthDictionary.Add(FocalLength.Normal, normalFocal);
        focalLengthDictionary.Add(FocalLength.Portrait, portraitFocal);
        focalLengthDictionary.Add(FocalLength.Tele, teleFocal);
    }

    void UpdateViewport()
    // Apply changes
    {
        currentAspectRatio = aspectRatioDictionary[aspectRatio].x / aspectRatioDictionary[aspectRatio].y;

        float width = 0.5f;
        float height = Mathf.Lerp(0.0f, width, Mathf.InverseLerp(0.0f, aspectRatioDictionary[aspectRatio].x, aspectRatioDictionary[aspectRatio].y)); // Calculate viewport height based on the given aspect ratio in relation to the maximum of the width
        Vector2 newViewportSize = new Vector2(width, height); // Create new Vector2 to resize the viewport panel
        Vector2 newIndicatorSize = new Vector2(newViewportSize.x, newViewportSize.y + 0.06f); // Copy size of viewport for interface with offset for text elements

        viewport.sizeDelta = newViewportSize;
        indicators.sizeDelta = newIndicatorSize;

        viewportCamera.aspect = currentAspectRatio;
    }

    void UpdateInterface()
    {
        List<float> cameraSettings = new List<float>();
        cameraSettings = AnimationRecorder.GetCameraSettings();
        framerateText.text = cameraSettings[0].ToString() + " fps";
        focalLengthText.text = cameraSettings[1].ToString() + " mm";
        aspectRatioText.text = aspectRatio.ToString() + " - " + currentAspectRatio.ToString();
    }

    public static void StartRecordingIndicator()
    {   
        recordingDot.color = recordingColor;
    }

    public static void StopRecordingIndicator()
    {
        recordingDot.color = defaultColor;
    }

    public void ResetViewportPosition()
    {
        Vector3 defaultPosition = vrCamera.transform.position + new Vector3(0f, -0.2f, 0.5f);
        virtualViewport.gameObject.transform.position = defaultPosition;
    }
}