using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class VRCTShortcuts : EditorWindow
{
    int mode = 0;
    string[] modeOptions = new string[] {"Default", "Recording", "Visualization"};
    float frameRate = 24.0f;
    Vector2 sensorSize = new Vector2(5.0f, 5.0f);
    Vector2 aspectRatio = new Vector2(1.0f, 1.0f);
    float focalLength = 25.0f;
    int animation = 0;
    string[] animationOptions;
    
    private bool recording = false;
    private GameObject rig;
    private GameObject virtualViewport;
    private Camera viewportCamera;
    private AspectRatioManager aspectRatioManager;
    private ModeManager modeManager;
    private ViewportManager viewportManager;
    private AnimationRecorder animationRecorder;
    private AnimationManager animationManager;
    private List<GameObject> animations;
    private List<GameObject> previews;
    private GameObject animationContainer;
    private GameObject previewContainer;

    [MenuItem("Tools/VRCTShortcuts")]

    public static void OpenWindow()
    {
        GetWindow(typeof(VRCTShortcuts));
    }

    private void OnGUI()
    {
        InitialSetup();

        EditorGUI.BeginChangeCheck();
        mode = EditorGUILayout.Popup("Mode", mode, modeOptions);
        if (EditorGUI.EndChangeCheck())
        {
            UpdateMode();
        }

        EditorGUILayout.Space();
    
        using (new EditorGUI.DisabledScope(mode != 1))
        {
            EditorGUI.BeginChangeCheck();
            frameRate = EditorGUILayout.FloatField("Framerate", frameRate);
            sensorSize = EditorGUILayout.Vector2Field("Sensor Size", sensorSize);
            aspectRatio = EditorGUILayout.Vector2Field("Aspect Ratio", aspectRatio);
            focalLength = EditorGUILayout.Slider("Focal Length", focalLength, 10.0f, 200.0f);

            if (EditorGUI.EndChangeCheck())
            {
                UpdateCameraSettings();
            }

            if (!recording)
            {
                if (GUILayout.Button("Start Recording"))
                {
                    StartRecording();
                }
            }
            else
            {
                if (GUILayout.Button("Stop Recording"))
                {
                    StopRecording();
                }
            }
        }

        using (new EditorGUI.DisabledScope(mode != 2))
        {
            EditorGUI.BeginChangeCheck();
            if (animations.Count != 0)
            {
                animation = EditorGUILayout.Popup("Animation", animation, animationOptions);
                if (EditorGUI.EndChangeCheck())
                {
                    UpdateAnimationPreview();
                }
            }
        }
    }

    private void InitialSetup()
    {
        rig = GameObject.FindGameObjectWithTag("Rig");
        virtualViewport = GameObject.FindGameObjectWithTag("VirtualViewport");
        viewportCamera = virtualViewport.GetComponent<Camera>();
        aspectRatioManager = virtualViewport.GetComponent<AspectRatioManager>();
        modeManager = rig.GetComponent<ModeManager>();
        viewportManager = rig.GetComponent<ViewportManager>();
        animationRecorder = rig.GetComponent<AnimationRecorder>();
        animationManager = rig.GetComponent<AnimationManager>();
        animationContainer = GameObject.FindGameObjectWithTag("LineContainer");
        previewContainer = GameObject.FindGameObjectWithTag("PreviewContainer");


        UpdateMode();
    }

    private void UpdateMode()
    {
        if (recording)
        {
            StopRecording();
        }
        if (mode == 0)
        {
            modeManager.interactionMode = ModeManager.InteractionMode.explore;
        }
        else if (mode == 1)
        {
            modeManager.interactionMode = ModeManager.InteractionMode.recording;
        }
        else if (mode == 2)
        {
            modeManager.interactionMode = ModeManager.InteractionMode.visualizing;
            Visualize();
        }
    }

    private void StartRecording()
    {
        recording = true;
        UpdateCameraSettings();
        animationRecorder.StartRecording();
        Debug.Log("Started Recording");
    }

    private void StopRecording()
    {
        recording = false;
        animationRecorder.StopRecording();
        Debug.Log("Stopped Recording");
    }

    private void Visualize()
    {
        if (CheckPlayMode())
        {
            animationManager.BuildAnimations();
            animations = new List<GameObject>();
            previews = new List<GameObject>();
    
            foreach (Transform item in animationContainer.transform)
            {
                animations.Add(item.gameObject);
            }
    
            foreach (Transform item in previewContainer.transform)
            {
                previews.Add(item.gameObject);
            }
    
            animationOptions = new string[animations.Count];
            for (int index = 0; index < animations.Count; index++)
            {
                animationOptions[index] = animations[index].name;
            }
    
            Debug.Log("Visualizing");
        }
    }

    private void UpdateCameraSettings()
    {
        Debug.Log("Updated Camera");
        animationRecorder.SetFramerate(frameRate);
        viewportCamera.sensorSize = sensorSize;
        aspectRatioManager.aspectRatio = aspectRatio.x / aspectRatio.y;
        viewportCamera.focalLength = focalLength;
    }

    private void UpdateAnimationPreview()
    {
        if (CheckPlayMode())
        {
            animationManager.ToggleVisibility(animations[animation].name);
            animationManager.UpdatePreviewAnimation(animations[animation].name);
        }
    }

    private bool CheckPlayMode()
    {
        if (EditorApplication.isPlaying)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
