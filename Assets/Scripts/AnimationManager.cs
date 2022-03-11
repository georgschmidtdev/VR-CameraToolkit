using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class AnimationManager : MonoBehaviour
{
    public GameObject extractorPrefab;
    public GameObject visualizerPrefab;
    public GameObject animationContainer;
    public AnimationClip exampleClip;
    public float lineWidth = 0.05f;
    private GameObject extractor;
    private string qualifier = "*.saf";
    private string animationDirectory;
    private bool scriptIsEnabled = false;
    private DirectoryInfo directory;
    private List<string> animationFiles;
    private List<AnimationClip> animationClips;
    private AnimationClip currentAnimationClip;
    private List<GameObject> visualizers;
    private bool overrideExistingCoordinates = false;
    private bool refreshAnimation = false;
    private Dictionary<string, List<Vector3>> coordinateDictionary;
    private List<string> serializedClips;

    // Start is called before the first frame update
    void Start()
    {
        animationDirectory = Application.persistentDataPath + "/RecordedAnimations/";
        if (!Directory.Exists(animationDirectory))
        {   
            Directory.CreateDirectory(animationDirectory);
        }

        animationClips = new List<AnimationClip>();
        serializedClips = new List<string>();
        visualizers = new List<GameObject>();
        extractor = Instantiate(extractorPrefab);
        directory = new DirectoryInfo(animationDirectory);
        coordinateDictionary = new Dictionary<string, List<Vector3>>();
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
            Debug.Log(animationClips.Count);
            ExtractCoordinates();
        }

        if (Input.GetKeyDown("n"))
        {
            AnimationCurve curve = AnimationUtility.
            SerializationManager.SaveFile(exampleClip);

            /**foreach (var item in animationClips)
            {
                string serializedClip = SerializationManager.Serialize(item);
                Debug.Log(serializedClip);
                serializedClips.Add(serializedClip);
            }**/
        }
    }  

    void IndexAnimations()
    {
        ResetFileIndex();
        
        foreach (var file in Directory.GetFiles(animationDirectory, qualifier, SearchOption.AllDirectories))
        {
            string parentDirectory = new DirectoryInfo(file).Parent.Name + "/";
            string path = Path.Combine(parentDirectory, Path.GetFileNameWithoutExtension(file));
            animationFiles.Add(path);
            LoadAnimation(path);
        }
    }

    void LoadAnimation(string path)
    {
        currentAnimationClip = Resources.Load<AnimationClip>(path);
        currentAnimationClip.legacy = false;
        animationClips.Add(currentAnimationClip);
    }

    void ResetFileIndex()
    {
        animationFiles = null;
        animationClips = null;
        animationFiles = new List<string>();
        animationClips = new List<AnimationClip>();
    }

    void ExtractCoordinates()
    {
        Animator animator = extractor.GetComponent<Animator>();
        Avatar avatar = AvatarBuilder.BuildGenericAvatar(extractor, "");
        animator.avatar = avatar;
        AnimatorOverrideController overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = overrideController;
        List<Vector3> coordinates = new List<Vector3>();

        foreach (var clip in animationClips)
        {
            overrideController["DefaultAnimation"] = clip;

            for (float time = 0.0f; time < clip.length; time += (1.0f / clip.frameRate))
            {
                float normalizedTime = Mathf.InverseLerp(0.0f, clip.length, time);
                
                animator.SetTarget(AvatarTarget.Body, normalizedTime);
                animator.Update(time);
                coordinates.Add(animator.targetPosition);
            }

            VisualizeAnimation(clip.name, coordinates);
            UpdateDictionary(clip.name, coordinates);
        }
    }

    void UpdateDictionary(string name, List<Vector3> coordinates)
    {
        if (coordinateDictionary.ContainsKey(name))
        {
            if(overrideExistingCoordinates)
            {
                coordinateDictionary.Remove(name);
                coordinateDictionary.Add(name, coordinates);
                overrideExistingCoordinates = false;
            }
            
            else
            {
                Debug.Log("Entry for " + name + " already exists");
            }
        }
        else
        {
            coordinateDictionary.Add(name, coordinates);
            overrideExistingCoordinates = false;
        }
    }

    void VisualizeAnimation(string name, List<Vector3> coordinates)
    {
        Transform child = animationContainer.transform.Find(name);

        if (child != null)
        {
            if (refreshAnimation)
            {
                Destroy(child.gameObject);
                GameObject currentVisualizer;
                currentVisualizer = Instantiate(visualizerPrefab, animationContainer.transform, true);
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
            else
            {
                Debug.Log("Visualizer for " + name + " already exists");
            }
        }

        else
        {
            GameObject currentVisualizer;
            currentVisualizer = Instantiate(visualizerPrefab, animationContainer.transform, true);
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

    public void DisableScript()
    {
        scriptIsEnabled = false;
    }

    public void EnableScript()
    {
        scriptIsEnabled = true;
    }
}
