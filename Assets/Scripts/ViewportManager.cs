using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ViewportManager : MonoBehaviour
{
    public Camera viewportCamera;
    public Canvas viewportCanvas;
    public Canvas indicatorCanvas;
    public GameObject recordingIndicator;
    public GameObject framerateIndicator;
    public GameObject focalLengthIndicator;
    public GameObject aspectRatioIndicator;
    public enum AspectRatio {Standard, Square, HDTV, Still35, Widescreen, Film35, Panavision};
    public AspectRatio aspectRatio;
    public float defaultFocalLength = 35f;
    public float customFocalLength = 35f;

    private RectTransform viewport;
    private RectTransform indicators;
    private Image recordingDot;
    private TextMeshProUGUI framerateText;
    private TextMeshProUGUI focalLengthText;
    private TextMeshProUGUI aspectRatioText;
    private Color defaultColor = Color.white;
    private Color recordingColor = Color.red;
    private Vector2 standardRatio = new Vector2(4.0f, 3.0f);
    private Vector2 squareRatio = new Vector2(1.0f, 1.0f);
    private Vector2 hdtvRatio = new Vector2(16.0f, 9.0f);
    private Vector2 still35Ratio = new Vector2(3.0f, 2.0f);
    private Vector2 widescreenRatio = new Vector2(14.0f, 9.0f);
    private Vector2 film35Ratio = new Vector2(1.85f, 1.0f);
    private Vector2 panavisionRatio = new Vector2(2.39f, 1.0f);
    private Dictionary<AspectRatio, Vector2> aspectRatioDictionary;
    private float currentAspectRatio = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        aspectRatioDictionary = new Dictionary<AspectRatio, Vector2>();
        aspectRatioDictionary.Add(AspectRatio.Standard, standardRatio);
        aspectRatioDictionary.Add(AspectRatio.Square, squareRatio);
        aspectRatioDictionary.Add(AspectRatio.HDTV, hdtvRatio);
        aspectRatioDictionary.Add(AspectRatio.Still35, still35Ratio);
        aspectRatioDictionary.Add(AspectRatio.Widescreen, widescreenRatio);
        aspectRatioDictionary.Add(AspectRatio.Film35, film35Ratio);
        aspectRatioDictionary.Add(AspectRatio.Panavision, panavisionRatio);

        viewport = viewportCanvas.GetComponent<RectTransform>();
        indicators = indicatorCanvas.GetComponent<RectTransform>();

        recordingDot = recordingIndicator.GetComponent<Image>();
        framerateText = framerateIndicator.GetComponent<TextMeshProUGUI>();
        focalLengthText = focalLengthIndicator.GetComponent<TextMeshProUGUI>();
        aspectRatioText = aspectRatioIndicator.GetComponent<TextMeshProUGUI>();

        UpdateViewport();
        UpdateInterface();
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

    private void UpdateViewport()
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

    private void UpdateInterface()
    {
        List<float> cameraSettings = new List<float>();
        cameraSettings = AnimationRecorder.GetCameraSettings();
        framerateText.text = cameraSettings[0].ToString() + " fps";
        focalLengthText.text = cameraSettings[1].ToString() + " mm";
        aspectRatioText.text = aspectRatio.ToString() + " - " + currentAspectRatio.ToString();
    }

    public void StartRecordingIndicator()
    {   
        recordingDot.color = recordingColor;
    }

    public void StopRecordingIndicator()
    {
        recordingDot.color = defaultColor;
    }
}