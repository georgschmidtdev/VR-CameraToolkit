using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class AnimationManager : MonoBehaviour
{
    public XRNode inputDevice;
    public InputHelpers.Button activationButton;
    public GameObject extractorPrefab;
    public GameObject extractorContainer;
    public GameObject visualizerPrefab;
    public GameObject lineContainer;
    public GameObject previewPrefab;
    public GameObject previewContainer;
    public GameObject playbackPanel;
    public GameObject animationBrowser;
    public GameObject animationBrowserList;
    public GameObject listEntryPrefab;
    public GameObject cameraPreviewCanvas;
    public RenderTexture cameraPreviewCanvasRenderTexture;
    public RenderTexture defaultRenderTexture;
    public float lineWidth = 0.025f;
    public TMP_Dropdown previewDropdown;

    private bool scriptIsEnabled = false;
    private bool wasActivated = false;
    private GameObject extractor;
    private string qualifier = "*.anim";
    private string saveDirectory;
    private string sessionDirectory;
    private GameObject currentAnimationPreview;
    private Dictionary<AnimationClip, List<Vector3>> clipDictionary;

    // Start is called before the first frame update
    void Start()
    {
        DisableScript();
        clipDictionary = new Dictionary<AnimationClip, List<Vector3>>();
    }

    // Update is called once per frame
    void Update()
    {
        if (scriptIsEnabled)
        {
            CheckIfActive();
        }
    }

    void CheckIfActive()
    // Unitys integrated GetKeyDown() function is not available for XR-Controller based inputs
    // This function emulates the same behaviour for a given (InputDevice device) and button (activationButton)
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(inputDevice);

        InputHelpers.IsPressed(device, activationButton, out bool isPressed);
        
        if (isPressed && !wasActivated)
        {
            wasActivated = true;
            BuildAnimations();
        }
        
        else if (!isPressed && wasActivated)
        {
            wasActivated = false;
        }
    }

    public void BuildAnimations()
    {
        ResetAnimationIndex(); // Clear already indexed files to prevent duplicates
        IndexAnimations();
        AddAnimationPreview();
        VisualizeAnimation();
        UpdateAnimationBrowser();
    }

    void IndexAnimations()
    // Search the Resources folder for existing .anim AnimationClips
    {
        saveDirectory = SessionManager.GetSaveDirectory();
        sessionDirectory = SessionManager.GetSessionDirectory();
        
        foreach (var file in Directory.GetFiles(saveDirectory, qualifier, SearchOption.AllDirectories))
        // Iterate through each file in the directory and all its sub-directories
        {
            // Prepare the files path for loading
            string parentDirectory = Path.Combine(new DirectoryInfo(sessionDirectory).Parent.Name + "/", new DirectoryInfo(file).Parent.Name + "/");
            string path = Path.Combine(parentDirectory, Path.GetFileNameWithoutExtension(file));
            path = path.Replace("/", "\\");
            LoadAnimation(path);
        }
    }

    void LoadAnimation(string path)
    // Load files located at path
    {
        AnimationClip currentAnimationClip = Resources.Load<AnimationClip>(path);
        currentAnimationClip.legacy = false; // Disable legacy mode for loaded AnimationClip to ensure compatibility with Animator-component
        ExtractCoordinates(currentAnimationClip);
    }

    void ExtractCoordinates(AnimationClip clip)
    // Extracts keyframed coordinates from AnimationClips to use for LineRenderer later
    {
        
        List<Vector3> coordinates = new List<Vector3>();
        extractor = Instantiate(extractorPrefab, extractorContainer.transform, true);
        Animator animator = extractor.GetComponent<Animator>(); // Use instantiated prefab to hold the current AnimationClip
        AnimatorOverrideController overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = overrideController;
        overrideController["DefaultAnimation"] = clip; // Replace placeholder Animation with current AnimationClip
        float frameDelta = 1.0f / clip.frameRate;

        for (float time = 0.0f; time <= clip.length; time += frameDelta)
        // Step through Animation one timestep at a time
        {
            float normalizedTime = Mathf.InverseLerp(0.0f, clip.length, time);
            
            animator.SetTarget(AvatarTarget.Root, normalizedTime);
            animator.Update(frameDelta); // Update Animator to current timestep
            coordinates.Add(extractor.gameObject.transform.localPosition); // Save current location
        }
        
        clipDictionary.Add(clip, coordinates);  // Add the keyvaluepair to the dictionary
        Destroy(extractor);
    }

    void AddAnimationPreview()
    {
        foreach (var item in clipDictionary)
        {
            GameObject currentPreview;
            currentPreview = Instantiate(previewPrefab, previewContainer.transform, true);
            currentPreview.name = item.Key.name;

            Animator previewAnimator = currentPreview.GetComponent<Animator>();
            previewAnimator.StopPlayback();
            AnimatorOverrideController previewOverrideController = new AnimatorOverrideController(previewAnimator.runtimeAnimatorController);
            previewAnimator.runtimeAnimatorController = previewOverrideController;
            previewOverrideController["DefaultAnimation"] = item.Key;

            AspectRatioManager currentAspectRatioManager = currentPreview.GetComponent<AspectRatioManager>();
            currentPreview.GetComponent<Camera>().aspect = currentAspectRatioManager.GetAspectRatio();
        }
    }

    private void UpdateAnimationBrowser()
    // Populate animation browser for visualized animations
    {
        foreach (var item in clipDictionary)
        {
            TMP_Dropdown.OptionData newOption = new TMP_Dropdown.OptionData();
            newOption.text = item.Key.name;
            previewDropdown.options.Add(newOption);
        }

        previewDropdown.RefreshShownValue();
    }

    void VisualizeAnimation()
    // Create new LineRenderer object for given animation
    {
        foreach (var item in clipDictionary)
        {
            Vector3 targetPosition = CalculateBounds(item.Value);

            Transform child = lineContainer.transform.Find(item.Key.name);
            if (child != null)
            // Check if LineRenderer for clip already exists in the scene
            {
                Destroy(child.gameObject); // Remove existing object
            }

            GameObject currentVisualizer = Instantiate(visualizerPrefab, lineContainer.transform, true); // Instantiate visualizer prefab
            currentVisualizer.gameObject.SetActive(false);
            currentVisualizer.name = item.Key.name; // Assign name of AnimationClip to instantiated object

            // Setup for LineRenderer
            LineRenderer line = currentVisualizer.GetComponent<LineRenderer>();
            line.startWidth = lineWidth;
            line.endWidth = lineWidth;
            line.positionCount = item.Value.Count; // Set point count of line depending on given clip
            
            for (int index = 0; index < item.Value.Count; index++)
            // Add every position to line
            {
                line.SetPosition(index, item.Value[index]);
            }

            SetLineColor(line);
            SetAnimationInfo(currentVisualizer, item.Key, line, targetPosition);
        }
    }

    private Vector3 CalculateBounds(List<Vector3> coordinates)
    // Calculate position for the information canvas of AnimationVisualizers
    // Get "bounding box" by finding minimum and maximum values for recorded coordinates
    {
        List<float> xCoordinates = new List<float>();
        List<float> yCoordinates = new List<float>();
        List<float> zCoordinates = new List<float>();

        foreach (var item in coordinates)
        // Translate each axis into its own list
        {
            xCoordinates.Add(item.x);
            yCoordinates.Add(item.y);
            zCoordinates.Add(item.z);
        }

        // Get minimum and maximum values
        float xMin = xCoordinates.Min();
        float xMax = xCoordinates.Max();
        float yMax = yCoordinates.Max();
        float zMin = zCoordinates.Min();
        float zMax = zCoordinates.Max();

        // Calculate average of X and Z
        float xPosition = (xMin + xMax) / 2;
        float zPosition = (zMin + zMax) / 2;

        Vector3 targetPosition = new Vector3(xPosition, yMax, zPosition);

        return targetPosition;
    }

    private void SetAnimationInfo(GameObject visualizer, AnimationClip clip, LineRenderer line, Vector3 target)
    // Enter information about AnimationClip into canvas
    {
        Transform constraintPivot = visualizer.gameObject.transform.GetChild(0);
        Transform canvas = constraintPivot.gameObject.transform.GetChild(0);
        TextMeshProUGUI name = canvas.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI length = canvas.gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI frameRate = canvas.gameObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        Image background = canvas.gameObject.transform.GetChild(3).GetComponent<Image>();

        name.text = clip.name;
        length.text = clip.length.ToString() + " Seconds";
        frameRate.text = clip.frameRate.ToString() + " fps";
        background.color = GetBlendedColor(line);

        canvas.transform.position = target;
    }

    private void SetLineColor(LineRenderer line)
    {
        line.startColor = GetRandomColor();
        line.endColor = OffsetColorHue(line.startColor);
    }

    private Color GetRandomColor()
    {
        Color randomColor = new Color 
        (
            Random.Range(0.0f, 1.0f),
            Random.Range(0.0f, 1.0f),
            Random.Range(0.0f, 1.0f),
            1.0f
        );

        return randomColor;
    }

    private Color OffsetColorHue(Color input)
    {
        Color offsetColor;
        float hue;
        float saturation;
        float value;

        Color.RGBToHSV(input, out hue, out saturation, out value);

        hue += Random.Range(- hue, 1 - hue);

        offsetColor = Color.HSVToRGB(hue, saturation, value);

        return offsetColor;
    }

    private Color GetBlendedColor(LineRenderer line)
    {
        float colorR = line.startColor.r + line.endColor.r / 2.0f;
        float colorG = line.startColor.g + line.endColor.g / 2.0f;
        float colorB = line.startColor.b + line.endColor.b / 2.0f;

        Color blendedColor = new Color(colorR, colorG, colorB, 0.5f);

        return blendedColor;
    }

    void ResetAnimationIndex()
    {
        foreach (var item in clipDictionary)
        {
            DeleteAnimationVisualizer(item.Key.name);
            DeleteAnimationPreview(item.Key.name);
            DeleteBrowserEntry(item.Key.name);
        }

        // Reset all indexed animations
        clipDictionary = null;
        clipDictionary = new Dictionary<AnimationClip, List<Vector3>>();
    }

    public void ToggleVisibility(string name)
    {   
        foreach (Transform item in lineContainer.gameObject.transform)
        {
            if(item.name != name)
            {
                item.gameObject.SetActive(false);
            }
            else
            {
                item.gameObject.SetActive(!item.gameObject.activeSelf);

                if (item.gameObject.activeSelf == true)
                {
                    cameraPreviewCanvas.SetActive(true);
                }
                else
                {
                    cameraPreviewCanvas.SetActive(false);
                }
            }
        }        

        UpdatePreviewAnimation(name);
    }

    public void DeleteAnimation(string name)
    {
        DeleteDictionaryEntry(name);
        DeleteAnimationVisualizer(name);
        DeleteAnimationPreview(name);
        DeleteBrowserEntry(name);
        DeleteAnimationFile(name);
        UpdateAnimationBrowser();
    }

    void DeleteDictionaryEntry(string name)
    {
        AnimationClip clipToDelete = new AnimationClip();
        foreach (var item in clipDictionary)
        {
            if (item.Key.name == name)
            {
                clipToDelete = item.Key;
            }
        }

        clipDictionary.Remove(clipToDelete);
    }

    void DeleteAnimationVisualizer(string name)
    // Delete visualizer
    {
        foreach (Transform item in lineContainer.gameObject.transform)
        {
            if (item.gameObject.name == name)
            {
                Destroy(item.gameObject);
            }
        }
    }

    void DeleteAnimationPreview(string name)
    // Remove animation from camera preview
    {
        foreach (Transform item in previewContainer.gameObject.transform)
        {
            if (item.name == name)
            {
                Destroy(item.gameObject);
            }
        }
    }

    void DeleteBrowserEntry(string name)
    {
        Destroy(animationBrowserList.gameObject.transform.Find(name).gameObject);
    }

    void DeleteAnimationFile(string name)
    // Delete file from disk
    {
        string deleteQualifier = name + ".*";
        
        foreach (var file in Directory.GetFiles(saveDirectory, deleteQualifier, SearchOption.AllDirectories))
        {
            File.Delete(file.ToString());
        }
    }

    public void UpdatePreviewAnimation(string name)
    // Set camera preview to active animation
    {
        float width = 0.5f;
        foreach (Transform item in previewContainer.gameObject.transform)
        {
            item.gameObject.GetComponent<Camera>().targetTexture = defaultRenderTexture;

            if (item.gameObject.name == name)
            {
                currentAnimationPreview = item.gameObject;
                currentAnimationPreview.GetComponent<Camera>().targetTexture = cameraPreviewCanvasRenderTexture;

                float height = width / currentAnimationPreview.GetComponent<AspectRatioManager>().GetAspectRatio();
                Vector2 newPreviewSize = new Vector2(width, height);
                cameraPreviewCanvas.GetComponent<RectTransform>().sizeDelta = newPreviewSize;
            }
        }
    }

    public void PlayAnimation()
    {
        currentAnimationPreview.GetComponent<Animator>().speed = 1.0f;
    }

    public void PauseAnimation()
    {
        currentAnimationPreview.GetComponent<Animator>().speed = 0.0f;    
    }

    public void RestartAnimation()
    {
        PauseAnimation();
        currentAnimationPreview.GetComponent<Animator>().Play("DefaultAnimation", -1, 0.0f);
    }

    public void DisableScript()
    {
        scriptIsEnabled = false;
        lineContainer.gameObject.SetActive(false);
        animationBrowser.gameObject.SetActive(false);
        cameraPreviewCanvas.gameObject.SetActive(false);
    }

    public void EnableScript()
    {
        scriptIsEnabled = true;
        BuildAnimations();
        lineContainer.gameObject.SetActive(true);
        animationBrowser.gameObject.SetActive(true);
    }
}
