using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class VisualizationManager : MonoBehaviour
{
    public GameObject prefab;
    private string qualifier = "*.anim";
    private string animationDirectory = "Assets/Resources/Recordings/";
    private bool scriptIsEnabled = false;
    private DirectoryInfo directory;
    private List<string> animationFiles;
    private List<AnimationClip> animationClips;
    private AnimationClip currentAnimationClip;
    private GameObject currentInstance;
    private Dictionary<string, List<Vector3>> coordinateDictionary;

    // Start is called before the first frame update
    void Start()
    {
        directory = new DirectoryInfo(animationDirectory);
        coordinateDictionary = new Dictionary<string, List<Vector3>>();
        IndexAnimations();
    }

    // Update is called once per frame
    void Update()
    {
        //if (scriptIsEnabled)
        //{
        //    
        //}

        if (Input.GetKeyDown("m"))
        {
            Debug.Log(animationClips.Count);
            ExtractCoordinates();
        }
    }  

    void IndexAnimations()
    {
        ResetFileIndex();
        
        foreach (var file in directory.GetFiles(qualifier))
        {
            string parentDirectory = new DirectoryInfo(file.FullName).Parent.Name + "/";
            string path = Path.Combine(parentDirectory, Path.GetFileNameWithoutExtension(file.FullName));
            Debug.Log(path);
            animationFiles.Add(path);
            LoadAnimation(path);
        }
    }

    void LoadAnimation(string path)
    {
        currentAnimationClip = Resources.Load<AnimationClip>(path);
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
        currentInstance = null;

        foreach (var clip in animationClips)
        {
            currentInstance = Instantiate(prefab);
            Animation animationComponent = currentInstance.GetComponent<Animation>();
            Animator animatorComponent = currentInstance.GetComponent<Animator>();
            animationComponent.AddClip(clip, clip.name);
            List<Vector3> instanceCoordinates = new List<Vector3>();
            

            for (float i = 0; i <= clip.length; i += (1.0f / clip.frameRate))
            {
                float normalizedTime = Mathf.InverseLerp(0f, clip.length, i);
                Debug.Log(i);
                animatorComponent.SetTarget(AvatarTarget.Root, normalizedTime);
                animatorComponent.Play(0, -1, normalizedTime);
                animatorComponent.Update(i);
                Vector3 currentCoordinates = animatorComponent.targetPosition;
                instanceCoordinates.Add(currentCoordinates);
                Debug.Log(currentCoordinates.x + ", " + currentCoordinates.y + ", "+ currentCoordinates.z);
            }

            coordinateDictionary.Add(clip.name, instanceCoordinates);  
            Destroy(currentInstance);
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
