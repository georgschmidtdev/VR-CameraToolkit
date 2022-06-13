using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class VRCTShortcuts : EditorWindow
{
    int mode = 0;
    string[] modeOptions = new string[] {"Exploration", "Recording", "Visualization"};
    bool overrideActiveObject = false;
    int activeObject = 0;
    string[] overrideOptions;
    int animation = 0;
    string [] animationOptions;
    float frameRate;
    Vector2 aspectRatio = new Vector2(16.0f, 9.0f);
    float focalLength;
    
    private bool recording = false;
    private GameObject index;
    private GameObjectIndex gameObjectIndex;
    private GameObject rig;
    private GameObject virtualViewport;
    private Camera viewportCamera;
    private AspectRatioManager aspectRatioManager;
    private AnimationRecorder animationRecorder;
    private AnimationManager animationManager;
    private ModeManager modeManager;
    private GameObject lineContainer;

    [MenuItem("Tools/VRCTShortcuts")]

    public static void OpenWindow()
    {
        GetWindow(typeof(VRCTShortcuts));
    }

    private void OnGUI()
    {
        InitialSetup();

        EditorGUILayout.Space(10);

        if (!CheckPlayMode())
        {
            EditorGUILayout.LabelField("ENTER PLAY MODE.");
        }
        else if (CheckPlayMode() && !recording)
        {
            EditorGUILayout.LabelField(" ");
        }
        else if (CheckPlayMode() && recording)
        {
            EditorGUILayout.LabelField("RECORDING...");
        }

        EditorGUILayout.Space(20);

        using (new EditorGUI.DisabledScope(!CheckPlayMode()))
        {
            EditorGUI.BeginChangeCheck();
            mode = EditorGUILayout.Popup("Interaction Mode", mode, modeOptions);
            if (EditorGUI.EndChangeCheck())
            {
                SwitchMode();
            }

            using (new EditorGUI.DisabledScope(mode != 1))
            {
                using (new EditorGUI.DisabledScope(recording))
                {
                    EditorGUI.BeginChangeCheck();
                    overrideActiveObject = EditorGUILayout.Toggle("Override Active Object", overrideActiveObject);
                    using (new EditorGUI.DisabledScope(!overrideActiveObject))
                    {
                        activeObject = EditorGUILayout.Popup("Active Object", activeObject, overrideOptions);
                    }
                    if (EditorGUI.EndChangeCheck())
                    {
                        UpdateObjectOverride();
                    }

                    EditorGUILayout.Space(20);

                    EditorGUI.BeginChangeCheck();
                    frameRate = EditorGUILayout.FloatField("Framerate", frameRate);
                    aspectRatio = EditorGUILayout.Vector2Field("Aspect Ratio", aspectRatio);
                    focalLength = EditorGUILayout.Slider("Focal Length", focalLength, 10.0f, 200.0f);
                    if (EditorGUI.EndChangeCheck())
                    {
                        UpdateCameraSettings();
                    }
                }

                EditorGUILayout.Space(20);

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

            EditorGUILayout.Space(20);

            using (new EditorGUI.DisabledScope(mode != 2))
            {
                EditorGUI.BeginChangeCheck();
                animation = EditorGUILayout.Popup("Animation", animation, animationOptions);
                if (EditorGUI.EndChangeCheck())
                {
                    ToggleAnimation();
                }

                EditorGUILayout.Space(10);

                using (new EditorGUI.DisabledScope(animationOptions != null))
                {
                    if (GUILayout.Button("Delete Animation"))
                    {
                        DeleteAnimation();
                    }
                }

                
            }
        }
    }

    private void InitialSetup()
    {
        index = GameObject.FindGameObjectWithTag("Index");
        gameObjectIndex = index.GetComponent<GameObjectIndex>();
        rig = gameObjectIndex.rig;
        virtualViewport = gameObjectIndex.virtualViewport;
        viewportCamera = virtualViewport.GetComponent<Camera>();
        aspectRatioManager = virtualViewport.GetComponent<AspectRatioManager>();
        animationRecorder = rig.GetComponent<AnimationRecorder>();
        animationManager = rig.GetComponent<AnimationManager>();
        modeManager = rig.GetComponent<ModeManager>();
        lineContainer = gameObjectIndex.lineContainer;

        overrideOptions = new string[gameObjectIndex.activeObjectOverrides.Count];
        for (int index = 0; index < gameObjectIndex.activeObjectOverrides.Count; index++)
        {
            overrideOptions[index] = gameObjectIndex.activeObjectOverrides[index].name;
        }

        ResetMode();
    }

    private void StartRecording()
    {
        recording = true;
        UpdateCameraSettings();
        UpdateObjectOverride();
        animationRecorder.StartRecording();
    }

    private void StopRecording()
    {
        recording = false;
        animationRecorder.StopRecording();
    }

    private void SwitchMode()
    {
        if (mode == 0)
        {
            modeManager.interactionMode = ModeManager.InteractionMode.exploration;
            modeManager.ActivateScript(modeManager.interactionMode);
        }
        else if (mode == 1)
        {
            modeManager.interactionMode = ModeManager.InteractionMode.recording;
            modeManager.ActivateScript(modeManager.interactionMode);
        }
        else if (mode == 2)
        {
            modeManager.interactionMode = ModeManager.InteractionMode.visualizing;
            modeManager.ActivateScript(modeManager.interactionMode);

            List<GameObject> objectList = new List<GameObject>();
            foreach (Transform item in lineContainer.gameObject.transform)
            {
                objectList.Add(item.gameObject);
            }

            animationOptions = new string[objectList.Count];
            for (int index = 0; index < objectList.Count; index++)
            {
                animationOptions[index] = objectList[index].name;
            }
        }
    }

    private void ToggleAnimation()
    {
        animationManager.ToggleVisibility(animationOptions[animation]);
    }

    private void DeleteAnimation()
    {
        animationManager.DeleteAnimation(animationOptions[animation]);
    }

    private void UpdateCameraSettings()
    {
        animationRecorder.SetFramerate(frameRate);
        aspectRatioManager.aspectRatio = aspectRatio.x / aspectRatio.y;
        viewportCamera.focalLength = focalLength;
        Debug.Log("Updated Camera");
    }

    private void UpdateObjectOverride()
    {
        animationRecorder.overrideActiveObject = overrideActiveObject;
        animationRecorder.activeObject = gameObjectIndex.activeObjectOverrides[activeObject];
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

    private void ResetMode(){
        mode = 0;
    }
}
