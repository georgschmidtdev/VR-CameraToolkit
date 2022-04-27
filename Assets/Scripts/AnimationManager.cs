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
    public GameObject visualizerPrefab;
    public GameObject lineContainer;
    public GameObject animationBrowser;
    public GameObject animationBrowserList;
    public GameObject listEntryPrefab;
    public GameObject cameraPreview;
    public float lineWidth = 0.05f;

    private bool scriptIsEnabled = false;
    private bool wasActivated = false;
    private List<GameObject> animationList;
    private List<AnimationClip> animationClipList;
    private GameObject extractor;
    private string qualifier = "*.anim";
    private string saveDirectory;
    private string sessionDirectory;
    private AnimationClip currentAnimationClip;
    private List<GameObject> visualizers;
    private Dictionary<AnimationClip, List<Vector3>> clipDictionary;

    // Start is called before the first frame update
    void Start()
    {
        DisableScript();
        visualizers = new List<GameObject>();
        extractor = Instantiate(extractorPrefab);
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

    void BuildAnimations()
    {
        IndexAnimations();
        ExtractCoordinates();
        UpdateAnimationBrowser();
    }

    void IndexAnimations()
    // Search the Resources folder for existing .anim AnimationClips
    {
        saveDirectory = SessionManager.GetSaveDirectory();
        sessionDirectory = SessionManager.GetSessionDirectory();
        ResetFileIndex(); // Clear already indexed files to prevent dublicates
        
        foreach (var file in Directory.GetFiles(saveDirectory, qualifier, SearchOption.AllDirectories))
        // Iterate through each file in the directory and all its sub-directories
        {
            // Prepare the files path for loading
            string parentDirectory = Path.Combine(new DirectoryInfo(sessionDirectory).Parent.Name + "/", new DirectoryInfo(file).Parent.Name + "/");
            string path = Path.Combine(parentDirectory, Path.GetFileNameWithoutExtension(file));
            path = path.Replace("/", "\\");
            Debug.Log(path);
            LoadAnimation(path);
        }
    }

    void LoadAnimation(string path)
    // Load files located at path
    {
        currentAnimationClip = Resources.Load<AnimationClip>(path);
        currentAnimationClip.legacy = false; // Disable legacy mode for loaded AnimationClip to ensure compatibility with Animator-component
        animationClipList.Add(currentAnimationClip);
    }


    void ExtractCoordinates()
    // Extracts keyframed coordinates from AnimationClips to use for LineRenderer later
    {
        Animator animator = extractor.GetComponent<Animator>(); // Use instantiated prefab to hold the current AnimationClip
        AnimatorOverrideController overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        List<Vector3> coordinates = new List<Vector3>();
        Avatar avatar = AvatarBuilder.BuildGenericAvatar(extractor, "");
        animator.avatar = avatar;
        animator.runtimeAnimatorController = overrideController;

        foreach (var clip in animationClipList)
        {
            overrideController["DefaultAnimation"] = clip; // Replace placeholder Animation with current AnimationClip

            for (float time = 0.0f; time < clip.length; time += (1.0f / clip.frameRate))
            // Step through Animation one timestep at a time
            {
                float normalizedTime = Mathf.InverseLerp(0.0f, clip.length, time);
                
                animator.SetTarget(AvatarTarget.Body, normalizedTime);
                animator.Update(time); // Update Animator to current timestep
                coordinates.Add(animator.targetPosition); // Save current location
            }

            VisualizeAnimation(clip, coordinates);
            UpdateDictionary(clip, coordinates);
        }
    }

    void UpdateDictionary(AnimationClip clip, List<Vector3> coordinates)
    // Add the keyvaluepair to the dictionary
    {
        clipDictionary.Add(clip, coordinates);
    }

    private void UpdateAnimationBrowser()
    // Populate animation browser for visualized animations
    {
        animationList = new List<GameObject>();
        GameObject currentListEntry;
        TextMeshProUGUI currentEntryName;

        foreach (var item in clipDictionary)
        {
            currentListEntry = Instantiate(listEntryPrefab, parent: animationBrowserList.transform, true);
            currentEntryName = currentListEntry.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

            currentListEntry.name = item.Key.name;
            currentEntryName.text = item.Key.name;
            animationList.Add(currentListEntry);
        }

        for (int i = 0; i < animationList.Count; i++)
        {
            animationList[i].GetComponent<RectTransform>().localPosition = new Vector3(0, - i * 0.1f, 0);
        }
    }

    void VisualizeAnimation(AnimationClip clip, List<Vector3> coordinates)
    // Create new LineRenderer object for given animation
    {
        Vector3 targetPosition = CalculateBounds(coordinates);
        Transform child = lineContainer.transform.Find(clip.name);

        if (child != null)
        // Check if LineRenderer for clip already exists in the scene
        {
            Destroy(child.gameObject); // Remove existing object
        }

        GameObject currentVisualizer;
        currentVisualizer = Instantiate(visualizerPrefab, lineContainer.transform, true); // Instantiate prefab
        currentVisualizer.gameObject.SetActive(false); //
        currentVisualizer.name = clip.name; // Assign name of AnimationClip to instantiated objec
        // Setup for LineRenderer
        LineRenderer line = currentVisualizer.GetComponent<LineRenderer>();
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.positionCount = coordinates.Count; // Set point count of line depending on given clip

        for (int index = 0; index < coordinates.Count; index++)
        // Add every position to line
        {
            line.SetPosition(index, coordinates[index]);
        }

        SetLineColor(line);
        SetAnimationInfo(currentVisualizer, clip, line, targetPosition);
        visualizers.Add(currentVisualizer); // Store instance in list
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
        Transform canvas = visualizer.gameObject.transform.Find("InformationCanvas");
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

    void ResetFileIndex()
    {
        clipDictionary = null;
        animationClipList = null;
        clipDictionary = new Dictionary<AnimationClip, List<Vector3>>();
        animationClipList = new List<AnimationClip>();
    }

    public void ToggleVisibility(string name)
    {
        bool currentStatus = lineContainer.gameObject.transform.Find(name).gameObject.activeSelf;
        lineContainer.gameObject.transform.Find(name).gameObject.SetActive(!currentStatus);
    }

    public void DeleteAnimation(string name)
    {
        string deleteQualifier = name + ".*";
        
        foreach (var file in Directory.GetFiles(saveDirectory, deleteQualifier, SearchOption.AllDirectories))
        // Delete file from disk
        {
            Debug.Log(file.ToString());
            File.Delete(file.ToString());
        }

        BuildAnimations();
    }

    public void DisableScript()
    {
        scriptIsEnabled = false;
        lineContainer.gameObject.SetActive(false);
        animationBrowser.gameObject.SetActive(false);
        cameraPreview.gameObject.SetActive(false);
    }

    public void EnableScript()
    {
        scriptIsEnabled = true;
        lineContainer.gameObject.SetActive(true);
        animationBrowser.gameObject.SetActive(true);
        cameraPreview.gameObject.SetActive(true);
    }
}
