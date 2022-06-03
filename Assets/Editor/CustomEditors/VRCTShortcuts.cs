using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class VRCTShortcuts : EditorWindow
{
    float frameRate = 24.0f;
    Vector2 sensorSize = new Vector2(5.0f, 5.0f);
    Vector2 aspectRatio = new Vector2(1.0f, 1.0f);
    float focalLength = 25.0f;
    
    private bool recording = false;
    private GameObject index;
    private GameObjectIndex gameObjectIndex;
    private GameObject rig;
    private GameObject virtualViewport;
    private Camera viewportCamera;
    private AspectRatioManager aspectRatioManager;
    private AnimationRecorder animationRecorder;

    [MenuItem("Tools/VRCTShortcuts")]

    public static void OpenWindow()
    {
        GetWindow(typeof(VRCTShortcuts));
    }

    private void OnGUI()
    {
        InitialSetup();    
        using (new EditorGUI.DisabledScope(!CheckPlayMode()))
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

    private void UpdateCameraSettings()
    {
        Debug.Log("Updated Camera");
        animationRecorder.SetFramerate(frameRate);
        viewportCamera.sensorSize = sensorSize;
        aspectRatioManager.aspectRatio = aspectRatio.x / aspectRatio.y;
        viewportCamera.focalLength = focalLength;
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
