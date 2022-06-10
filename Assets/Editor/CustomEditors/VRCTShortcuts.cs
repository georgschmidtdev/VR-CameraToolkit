using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class VRCTShortcuts : EditorWindow
{
    bool overrideActiveObject = false;
    int activeObject = 0;
    string[] overrideOptions;
    float frameRate;
    Vector2 sensorSize = new Vector2(36.0f, 24.0f);
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
    private ModeManager modeManager;

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
            EditorGUILayout.LabelField("ENTER PLAY MODE TO RECORD.");
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
                sensorSize = EditorGUILayout.Vector2Field("Sensor Size", sensorSize);
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
        modeManager = rig.GetComponent<ModeManager>();

        overrideOptions = new string[gameObjectIndex.activeObjectOverrides.Count];
        for (int index = 0; index < gameObjectIndex.activeObjectOverrides.Count; index++)
        {
            overrideOptions[index] = gameObjectIndex.activeObjectOverrides[index].name;
        }
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

    private void UpdateCameraSettings()
    {
        animationRecorder.SetFramerate(frameRate);
        viewportCamera.sensorSize = sensorSize;
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
}
