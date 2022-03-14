using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class AnimationManager : MonoBehaviour
{
    public GameObject extractorPrefab;
    public GameObject visualizerPrefab;
    public GameObject lineContainer;
    public float lineWidth = 0.05f;
    private GameObject extractor;
    private string qualifier = "*.anim";
    private string animationDirectory;
    private bool scriptIsEnabled = false;
    private AnimationClip currentAnimationClip;
    private List<GameObject> visualizers;
    private bool overrideExistingCoordinates = false;
    private bool refreshAnimation = false;
    private Dictionary<AnimationClip, List<Vector3>> clipDictionary;

    // Start is called before the first frame update
    void Start()
    {
        visualizers = new List<GameObject>();
        extractor = Instantiate(extractorPrefab);
        clipDictionary = new Dictionary<AnimationClip, List<Vector3>>();
        IndexAnimations();
    }

    // Update is called once per frame
    void Update()
    {
        if (scriptIsEnabled)
        {
            
        }

        if (Input.GetKeyDown("m"))
        {
            ExtractCoordinates();
        }
    }  

    void IndexAnimations()
    // Search the Resources folder for existing .anim AnimationClips
    {
        ResetFileIndex(); // Clear already indexed files to prevent dublicates
        
        foreach (var file in Directory.GetFiles(animationDirectory, qualifier, SearchOption.AllDirectories))
        // Iterate through each file in the directory and all its sub-directories
        {
            // Prepare the files path for loading
            string parentDirectory = new DirectoryInfo(file).Parent.Name + "/";
            string path = Path.Combine(parentDirectory, Path.GetFileNameWithoutExtension(file));
            path = path.Replace("/", "\\");
            LoadAnimation(path);
        }
    }

    void LoadAnimation(string path)
    // Load files located at path
    {
        currentAnimationClip = Resources.Load<AnimationClip>(path);
        currentAnimationClip.legacy = false; // Disable legacy mode for loaded AnimationClip to ensure compatibility with Animator-component
        overrideExistingCoordinates = true;
        UpdateDictionary(currentAnimationClip, new List<Vector3>()); // Store animation in dictionary with temporary empty list of coordinates
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

        foreach (var clip in clipDictionary)
        {
            overrideController["DefaultAnimation"] = clip.Key; // Replace placeholder Animation with current AnimationClip

            for (float time = 0.0f; time < clip.Key.length; time += (1.0f / clip.Key.frameRate))
            // Step through Animation one timestep at a time
            {
                float normalizedTime = Mathf.InverseLerp(0.0f, clip.Key.length, time);
                
                animator.SetTarget(AvatarTarget.Body, normalizedTime);
                animator.Update(time); // Update Animator to current timestep
                coordinates.Add(animator.targetPosition); // Save current location
            }

            VisualizeAnimation(clip.Key, coordinates);
            UpdateDictionary(clip.Key, coordinates);
        }
    }

    void UpdateDictionary(AnimationClip clip, List<Vector3> coordinates)
    // Add the keyvaluepair to the dictionary
    {
        if (clipDictionary.ContainsKey(clip))
        // Check if entry already exists
        {
            if(overrideExistingCoordinates)
            // Replace keyvaluepair if true
            {
                clipDictionary.Remove(clip);
                clipDictionary.Add(clip, coordinates);
                overrideExistingCoordinates = false;
            }
            
            else
            {
                Debug.Log("Entry for " + name + " already exists");
            }
        }
        else
        // Add new entry
        {
            clipDictionary.Add(clip, coordinates);
            overrideExistingCoordinates = false;
        }
    }

    void VisualizeAnimation(AnimationClip clip, List<Vector3> coordinates)
    // Creates new LineRenderer object for given animation
    {
        Transform child = lineContainer.transform.Find(clip.name);

        if (child != null)
        // Check if LineRenderer for clip already exists in the scene
        {
            if (refreshAnimation)
            // Replace existing object with new LineRenderer
            {
                Destroy(child.gameObject); // Remove existing object

                GameObject currentVisualizer;
                currentVisualizer = Instantiate(visualizerPrefab, lineContainer.transform, true); // Instantiate prefab
                currentVisualizer.name = name; // Assign name of AnimationClip to instantiated object

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

                visualizers.Add(currentVisualizer); // Store instance in list
                refreshAnimation = false;
            }

            else
            {
                Debug.Log("Visualizer for " + clip.name + " already exists");
            }
        }

        else
        {
            GameObject currentVisualizer;
            currentVisualizer = Instantiate(visualizerPrefab, lineContainer.transform, true);
            currentVisualizer.name = name;

            LineRenderer line = currentVisualizer.GetComponent<LineRenderer>();
            line.startWidth = lineWidth;
            line.endWidth = lineWidth;
            line.positionCount = coordinates.Count;

            for (int index = 0; index < coordinates.Count; index++)
            {
                line.SetPosition(index, coordinates[index]);
            }

            visualizers.Add(currentVisualizer);
            refreshAnimation = false;
        }
    }
    void ResetFileIndex()
    {
        clipDictionary = null;
        clipDictionary = new Dictionary<AnimationClip, List<Vector3>>();
    }

    void ResetLineRenderers()
    {

    }

    public void DisableScript()
    {
        scriptIsEnabled = false;
    }

    public void EnableScript()
    {
        scriptIsEnabled = true;
    }
}
